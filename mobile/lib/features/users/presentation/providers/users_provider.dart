import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../auth/data/models/auth_models.dart';
import '../../data/repositories/users_repository.dart';

// ─── All users list ───────────────────────────────────────────────────────────

final usersListProvider = FutureProvider<List<UserModel>>((ref) async {
  return ref.read(usersRepositoryProvider).getAllUsers();
});

final activeUsersProvider = FutureProvider<List<UserModel>>((ref) async {
  return ref.read(usersRepositoryProvider).getActiveUsers();
});

// ─── Search state ─────────────────────────────────────────────────────────────

class UserSearchState {
  final String query;
  final bool? filterActive;
  final int page;
  final bool isLoading;
  final String? error;
  final PagedResultModel<UserModel>? result;

  const UserSearchState({
    this.query = '',
    this.filterActive,
    this.page = 1,
    this.isLoading = false,
    this.error,
    this.result,
  });

  UserSearchState copyWith({
    String? query,
    bool? filterActive,
    int? page,
    bool? isLoading,
    String? error,
    PagedResultModel<UserModel>? result,
  }) {
    return UserSearchState(
      query: query ?? this.query,
      filterActive: filterActive ?? this.filterActive,
      page: page ?? this.page,
      isLoading: isLoading ?? this.isLoading,
      error: error,
      result: result ?? this.result,
    );
  }
}

class UserSearchNotifier extends StateNotifier<UserSearchState> {
  final UsersRepository _repo;

  UserSearchNotifier(this._repo) : super(const UserSearchState()) {
    search();
  }

  Future<void> search({String? query, bool? isActive}) async {
    final q = query ?? state.query;
    state = state.copyWith(query: q, isLoading: true, error: null, page: 1);
    try {
      final result = await _repo.searchUsers(
        query: q.isEmpty ? null : q,
        isActive: isActive ?? state.filterActive,
      );
      state = state.copyWith(isLoading: false, result: result);
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e.toString());
    }
  }

  Future<void> nextPage() async {
    if (state.result == null || !state.result!.hasNext) return;
    final nextPage = state.page + 1;
    state = state.copyWith(isLoading: true, page: nextPage);
    try {
      final result = await _repo.searchUsers(
        query: state.query.isEmpty ? null : state.query,
        isActive: state.filterActive,
        page: nextPage,
      );
      // Append items
      final existing = state.result!.items;
      final merged = PagedResultModel<UserModel>(
        items: [...existing, ...result.items],
        totalCount: result.totalCount,
        page: result.page,
        pageSize: result.pageSize,
      );
      state = state.copyWith(isLoading: false, result: merged);
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e.toString());
    }
  }
}

final userSearchProvider =
    StateNotifierProvider<UserSearchNotifier, UserSearchState>((ref) {
  return UserSearchNotifier(ref.read(usersRepositoryProvider));
});

// ─── User form (create/update) ────────────────────────────────────────────────

class UserFormNotifier extends StateNotifier<AsyncValue<void>> {
  final UsersRepository _repo;

  UserFormNotifier(this._repo) : super(const AsyncValue.data(null));

  Future<bool> create({
    required String username,
    required String email,
    required String password,
    String? firstName,
    String? lastName,
  }) async {
    state = const AsyncValue.loading();
    try {
      await _repo.createUser(
        username: username,
        email: email,
        password: password,
        firstName: firstName,
        lastName: lastName,
      );
      state = const AsyncValue.data(null);
      return true;
    } catch (e, st) {
      state = AsyncValue.error(e, st);
      return false;
    }
  }

  Future<bool> update(String id, {
    String? firstName,
    String? lastName,
    String? email,
    bool? isActive,
  }) async {
    state = const AsyncValue.loading();
    try {
      await _repo.updateUser(id,
          firstName: firstName, lastName: lastName, email: email, isActive: isActive);
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
      await _repo.deleteUser(id);
      state = const AsyncValue.data(null);
      return true;
    } catch (e, st) {
      state = AsyncValue.error(e, st);
      return false;
    }
  }
}

final userFormProvider =
    StateNotifierProvider<UserFormNotifier, AsyncValue<void>>((ref) {
  return UserFormNotifier(ref.read(usersRepositoryProvider));
});
