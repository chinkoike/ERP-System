import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../auth/data/models/auth_models.dart';
import '../datasources/users_remote_datasource.dart';

final usersRepositoryProvider = Provider<UsersRepository>((ref) {
  return UsersRepository(ref.read(usersRemoteDataSourceProvider));
});

class UsersRepository {
  final UsersRemoteDataSource _ds;
  UsersRepository(this._ds);

  Future<List<UserModel>> getAllUsers() => _ds.getAllUsers();
  Future<List<UserModel>> getActiveUsers() => _ds.getActiveUsers();
  Future<UserModel> getUserById(String id) => _ds.getUserById(id);

  Future<PagedResultModel<UserModel>> searchUsers({
    String? query,
    bool? isActive,
    int page = 1,
    int pageSize = 20,
  }) =>
      _ds.searchUsers(query: query, isActive: isActive, page: page, pageSize: pageSize);

  Future<String> createUser({
    required String username,
    required String email,
    required String password,
    String? firstName,
    String? lastName,
    List<String>? roleIds,
  }) =>
      _ds.createUser({
        'username': username,
        'email': email,
        'password': password,
        if (firstName != null) 'firstName': firstName,
        if (lastName != null) 'lastName': lastName,
        if (roleIds != null) 'roleIds': roleIds,
      });

  Future<void> updateUser(String id, {
    String? firstName,
    String? lastName,
    String? email,
    bool? isActive,
  }) =>
      _ds.updateUser(id, {
        if (firstName != null) 'firstName': firstName,
        if (lastName != null) 'lastName': lastName,
        if (email != null) 'email': email,
        if (isActive != null) 'isActive': isActive,
      });

  Future<void> deleteUser(String id) => _ds.deleteUser(id);
  Future<bool> existsByUsername(String username) => _ds.existsByUsername(username);
  Future<bool> existsByEmail(String email) => _ds.existsByEmail(email);
}
