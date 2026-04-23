import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../../../../core/constants/api_constants.dart';
import '../datasources/auth_remote_datasource.dart';
import '../models/auth_models.dart';

// Same singleton options as dio_client.dart — MUST match
const _secureStorage = FlutterSecureStorage(
  aOptions: AndroidOptions(encryptedSharedPreferences: true),
);

final authRepositoryProvider = Provider<AuthRepository>((ref) {
  return AuthRepository(ref.read(authRemoteDataSourceProvider));
});

class AuthRepository {
  final AuthRemoteDataSource _dataSource;

  AuthRepository(this._dataSource);

  Future<AuthResponseModel> login(String username, String password) async {
    final response = await _dataSource.login(
      LoginRequestModel(username: username, password: password),
    );

    if (kDebugMode) {
      debugPrint('[AUTH REPO] Login response:');
      debugPrint('  isSuccess   = ${response.isSuccess}');
      debugPrint(
          '  accessToken = ${response.accessToken != null ? "✓ PRESENT (${response.accessToken!.length} chars)" : "✗ NULL"}');
      debugPrint(
          '  refreshToken= ${response.refreshToken != null ? "✓ PRESENT" : "✗ NULL"}');
      debugPrint('  user        = ${response.user?.username ?? "NULL"}');
    }

    if (response.isSuccess && response.accessToken != null) {
      await _saveTokens(response);

      // Verify the write actually worked
      if (kDebugMode) {
        final verify = await _secureStorage.read(key: StorageKeys.accessToken);
        debugPrint(
            '[AUTH REPO] Storage verify: ${verify != null ? "✓ token saved" : "✗ WRITE FAILED"}');
      }
    } else {
      if (kDebugMode) {
        debugPrint(
            '[AUTH REPO] ⚠️ Token NOT saved — isSuccess=${response.isSuccess}, accessToken=${response.accessToken}');
      }
    }

    return response;
  }

  Future<UserModel> register({
    required String username,
    required String email,
    required String password,
    String? firstName,
    String? lastName,
  }) async {
    return _dataSource.register(RegisterRequestModel(
      username: username,
      email: email,
      password: password,
      firstName: firstName,
      lastName: lastName,
    ));
  }

  Future<void> logout() async {
    final token = await _secureStorage.read(key: StorageKeys.refreshToken);
    if (token != null) {
      try {
        await _dataSource.logout(token);
      } catch (_) {}
    }
    await _secureStorage.deleteAll();
  }

  Future<bool> isAuthenticated() async {
    final token = await _secureStorage.read(key: StorageKeys.accessToken);
    if (kDebugMode)
      debugPrint('[AUTH REPO] isAuthenticated → ${token != null}');
    return token != null;
  }

  Future<String?> getStoredUsername() =>
      _secureStorage.read(key: StorageKeys.username);

  Future<void> _saveTokens(AuthResponseModel response) async {
    // Write sequentially — avoid concurrent writes on Android
    await _secureStorage.write(
        key: StorageKeys.accessToken, value: response.accessToken);
    if (response.refreshToken != null) {
      await _secureStorage.write(
          key: StorageKeys.refreshToken, value: response.refreshToken);
    }
    if (response.user != null) {
      await _secureStorage.write(
          key: StorageKeys.username, value: response.user!.username);
      await _secureStorage.write(
          key: StorageKeys.userId, value: response.user!.id);
      if (response.user!.roles.isNotEmpty) {
        await _secureStorage.write(
            key: StorageKeys.userRole, value: response.user!.roles.first);
      }
    }
  }
}
