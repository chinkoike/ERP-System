import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../constants/api_constants.dart';

// ─── Single shared storage instance ──────────────────────────────────────────
// IMPORTANT: FlutterSecureStorage must be a singleton.
// Creating multiple instances on Android can cause read failures.
const _secureStorage = FlutterSecureStorage(
  aOptions: AndroidOptions(encryptedSharedPreferences: true),
);

final dioProvider = Provider<Dio>((ref) {
  final dio = Dio(BaseOptions(
    baseUrl: ApiConstants.baseUrl,
    connectTimeout: ApiConstants.connectTimeout,
    receiveTimeout: ApiConstants.receiveTimeout,
    headers: {'Content-Type': 'application/json'},
  ));

  dio.interceptors.add(AuthInterceptor(dio));

  if (kDebugMode) {
    dio.interceptors.add(LogInterceptor(
      requestHeader: true, // show headers so we can see Authorization
      requestBody: true,
      responseBody: true,
      responseHeader: false,
      error: true,
      logPrint: (o) => debugPrint('[DIO] $o'),
    ));
  }

  return dio;
});

class AuthInterceptor extends QueuedInterceptor {
  // QueuedInterceptor serialises onRequest calls so async reads
  // complete before the next request goes out — fixes the race condition
  // where token reads returned null because Dio didn't await the future.

  final Dio _dio;

  AuthInterceptor(this._dio);

  static const _authPaths = [
    '/login',
    '/register',
    '/refresh-token',
    '/logout'
  ];

  bool _isAuthEndpoint(String path) => _authPaths.any((p) => path.contains(p));

  @override
  Future<void> onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) async {
    if (!_isAuthEndpoint(options.path)) {
      final token = await _secureStorage.read(key: StorageKeys.accessToken);

      if (kDebugMode) {
        debugPrint('[AUTH] ${options.method} ${options.path}');
        debugPrint(
            '[AUTH] token: ${token != null ? "✓ PRESENT" : "✗ NULL — will get 401"}');
      }

      if (token != null) {
        options.headers['Authorization'] = 'Bearer $token';
      }
    }
    handler.next(options);
  }

  @override
  Future<void> onError(
    DioException err,
    ErrorInterceptorHandler handler,
  ) async {
    if (err.response?.statusCode == 401) {
      if (kDebugMode)
        debugPrint('[AUTH] 401 received — attempting token refresh');

      final refreshed = await _tryRefreshToken();
      if (refreshed) {
        final token = await _secureStorage.read(key: StorageKeys.accessToken);
        final opts = err.requestOptions;
        opts.headers['Authorization'] = 'Bearer $token';
        try {
          final response = await _dio.fetch(opts);
          return handler.resolve(response);
        } catch (e) {
          if (kDebugMode) debugPrint('[AUTH] Retry after refresh failed: $e');
        }
      } else {
        // Refresh failed — clear storage so user lands on login
        await _secureStorage.deleteAll();
        if (kDebugMode) debugPrint('[AUTH] Refresh failed, storage cleared');
      }
    }
    handler.next(err);
  }

  Future<bool> _tryRefreshToken() async {
    try {
      final refreshToken =
          await _secureStorage.read(key: StorageKeys.refreshToken);
      if (refreshToken == null) {
        if (kDebugMode) debugPrint('[AUTH] No refresh token in storage');
        return false;
      }

      // Fresh Dio instance — avoids going through AuthInterceptor again
      final freshDio = Dio(BaseOptions(
        baseUrl: ApiConstants.baseUrl,
        headers: {'Content-Type': 'application/json'},
      ));

      final response = await freshDio.post(
        ApiConstants.refreshToken,
        data: {'refreshToken': refreshToken},
      );

      final data = response.data as Map<String, dynamic>;
      final newAccess = data['accessToken'] ??
          data['AccessToken'] ??
          data['token'] ??
          data['Token'];
      final newRefresh =
          data['refreshToken'] ?? data['RefreshToken'] ?? data['refresh_token'];

      if (newAccess != null) {
        await _secureStorage.write(
            key: StorageKeys.accessToken, value: newAccess.toString());
        if (newRefresh != null) {
          await _secureStorage.write(
              key: StorageKeys.refreshToken, value: newRefresh.toString());
        }
        if (kDebugMode) debugPrint('[AUTH] Token refreshed ✓');
        return true;
      }
      return false;
    } catch (e) {
      if (kDebugMode) debugPrint('[AUTH] _tryRefreshToken error: $e');
      return false;
    }
  }
}

// ─── API Exception ────────────────────────────────────────────────────────────

class ApiException implements Exception {
  final String message;
  final int? statusCode;

  ApiException({required this.message, this.statusCode});

  factory ApiException.fromDioError(DioException e) {
    final statusCode = e.response?.statusCode;
    final data = e.response?.data;
    String message;

    if (data is Map) {
      message = (data['message'] ??
              data['Message'] ??
              data['title'] ??
              data['Title'] ??
              '')
          .toString();
      if (message.isEmpty) message = 'Error $statusCode';
    } else if (data is String && data.isNotEmpty) {
      message = data;
    } else {
      message = switch (e.type) {
        DioExceptionType.connectionTimeout ||
        DioExceptionType.receiveTimeout =>
          'Connection timed out',
        DioExceptionType.connectionError => 'No internet connection',
        _ => e.message ?? 'Unknown error (${statusCode ?? '?'})',
      };
    }

    return ApiException(message: message, statusCode: statusCode);
  }

  @override
  String toString() => message;
}
