import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../data/models/auth_models.dart';
import '../../data/repositories/auth_repository.dart';

// ─── Auth State ───────────────────────────────────────────────────────────────

enum AuthStatus { initial, loading, authenticated, unauthenticated, error }

class AuthState {
  final AuthStatus status;
  final UserModel? user;
  final String? error;

  const AuthState({
    this.status = AuthStatus.initial,
    this.user,
    this.error,
  });

  AuthState copyWith({AuthStatus? status, UserModel? user, String? error}) {
    return AuthState(
      status: status ?? this.status,
      user: user ?? this.user,
      error: error,
    );
  }

  bool get isAuthenticated => status == AuthStatus.authenticated;
  bool get isLoading => status == AuthStatus.loading;
}

// ─── Auth Notifier ────────────────────────────────────────────────────────────

class AuthNotifier extends StateNotifier<AuthState> {
  final AuthRepository _repo;

  AuthNotifier(this._repo) : super(const AuthState()) {
    _checkAuth();
  }

  Future<void> _checkAuth() async {
    final isAuth = await _repo.isAuthenticated();
    state = state.copyWith(
      status: isAuth ? AuthStatus.authenticated : AuthStatus.unauthenticated,
    );
  }

  Future<bool> login(String username, String password) async {
    state = state.copyWith(status: AuthStatus.loading, error: null);
    try {
      final response = await _repo.login(username, password);
      if (response.isSuccess) {
        state = state.copyWith(
          status: AuthStatus.authenticated,
          user: response.user,
        );
        return true;
      } else {
        state = state.copyWith(
          status: AuthStatus.error,
          error: response.message ?? 'Login failed',
        );
        return false;
      }
    } catch (e) {
      state = state.copyWith(
        status: AuthStatus.error,
        error: e.toString(),
      );
      return false;
    }
  }

  Future<bool> register({
    required String username,
    required String email,
    required String password,
    String? firstName,
    String? lastName,
  }) async {
    state = state.copyWith(status: AuthStatus.loading, error: null);
    try {
      await _repo.register(
        username: username,
        email: email,
        password: password,
        firstName: firstName,
        lastName: lastName,
      );
      state = state.copyWith(status: AuthStatus.unauthenticated);
      return true;
    } catch (e) {
      state = state.copyWith(status: AuthStatus.error, error: e.toString());
      return false;
    }
  }

  Future<void> logout() async {
    await _repo.logout();
    state = const AuthState(status: AuthStatus.unauthenticated);
  }
}

// ─── Providers ────────────────────────────────────────────────────────────────

final authProvider = StateNotifierProvider<AuthNotifier, AuthState>((ref) {
  return AuthNotifier(ref.read(authRepositoryProvider));
});
