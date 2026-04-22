import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../../../../core/constants/api_constants.dart';
import '../datasources/auth_remote_datasource.dart';
import '../models/auth_models.dart';

final authRepositoryProvider = Provider<AuthRepository>((ref) {
  return AuthRepository(
    ref.read(authRemoteDataSourceProvider),
    const FlutterSecureStorage(),
  );
});

class AuthRepository {
  final AuthRemoteDataSource _dataSource;
  final FlutterSecureStorage _storage;

  AuthRepository(this._dataSource, this._storage);

  Future<AuthResponseModel> login(String username, String password) async {
    final response = await _dataSource.login(
      LoginRequestModel(username: username, password: password),
    );
    if (response.isSuccess && response.accessToken != null) {
      await _saveTokens(response);
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
    final token = await _storage.read(key: StorageKeys.refreshToken);
    if (token != null) {
      try {
        await _dataSource.logout(token);
      } catch (_) {}
    }
    await _clearTokens();
  }

  Future<bool> isAuthenticated() async {
    final token = await _storage.read(key: StorageKeys.accessToken);
    return token != null;
  }

  Future<String?> getStoredUsername() async {
    return _storage.read(key: StorageKeys.username);
  }

  Future<void> _saveTokens(AuthResponseModel response) async {
    await _storage.write(key: StorageKeys.accessToken, value: response.accessToken);
    if (response.refreshToken != null) {
      await _storage.write(key: StorageKeys.refreshToken, value: response.refreshToken);
    }
    if (response.user != null) {
      await _storage.write(key: StorageKeys.username, value: response.user!.username);
      await _storage.write(key: StorageKeys.userId, value: response.user!.id);
      if (response.user!.roles.isNotEmpty) {
        await _storage.write(key: StorageKeys.userRole, value: response.user!.roles.first);
      }
    }
  }

  Future<void> _clearTokens() async {
    await _storage.deleteAll();
  }
}
