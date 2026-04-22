import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../auth/data/models/auth_models.dart';
import '../../data/repositories/roles_repository.dart';

// All roles list
final rolesListProvider = FutureProvider<List<RoleModel>>((ref) {
  return ref.read(rolesRepositoryProvider).getAllRoles();
});

// Role form notifier (create / update / delete)
class RoleFormNotifier extends StateNotifier<AsyncValue<void>> {
  final RolesRepository _repo;

  RoleFormNotifier(this._repo) : super(const AsyncValue.data(null));

  Future<bool> create(String name, String? description) async {
    state = const AsyncValue.loading();
    try {
      await _repo.createRole(name, description);
      state = const AsyncValue.data(null);
      return true;
    } catch (e, st) {
      state = AsyncValue.error(e, st);
      return false;
    }
  }

  Future<bool> update(String id, String name, String? description) async {
    state = const AsyncValue.loading();
    try {
      await _repo.updateRole(id, name, description);
      state = const AsyncValue.data(null);
      return true;
    } catch (e, st) {
      state = AsyncValue.error(e, st);
      return false;
    }
  }

  Future<bool> delete(String id) async {
    state = const AsyncValue.loading();
    try {
      await _repo.deleteRole(id);
      state = const AsyncValue.data(null);
      return true;
    } catch (e, st) {
      state = AsyncValue.error(e, st);
      return false;
    }
  }
}

final roleFormProvider =
    StateNotifierProvider<RoleFormNotifier, AsyncValue<void>>((ref) {
  return RoleFormNotifier(ref.read(rolesRepositoryProvider));
});
