// ─── Roles Repository ─────────────────────────────────────────────────────────

import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../auth/data/models/auth_models.dart';
import '../datasources/roles_remote_datasource.dart';

final rolesRepositoryProvider = Provider<RolesRepository>((ref) {
  return RolesRepository(ref.read(rolesRemoteDataSourceProvider));
});

class RolesRepository {
  final RolesRemoteDataSource _ds;
  RolesRepository(this._ds);

  Future<List<RoleModel>> getAllRoles() => _ds.getAllRoles();
  Future<RoleModel> getRoleById(String id) => _ds.getRoleById(id);
  Future<String> createRole(String name, String? description) =>
      _ds.createRole(name, description);
  Future<void> updateRole(String id, String name, String? description) =>
      _ds.updateRole(id, name, description);
  Future<void> deleteRole(String id) => _ds.deleteRole(id);
  Future<bool> existsByName(String name) => _ds.existsByName(name);
}
