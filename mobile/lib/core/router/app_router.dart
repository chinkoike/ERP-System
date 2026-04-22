import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:mobile/features/auth/presentation/providers/auth_provider.dart';
import 'package:mobile/features/auth/presentation/screens/login_screen.dart';
import 'package:mobile/features/auth/presentation/screens/register_screen.dart';
import 'package:mobile/features/dashboard/presentation/dashboard_screen.dart';
import 'package:mobile/features/profile/presentation/screens/profile_screen.dart';
import 'package:mobile/features/roles/presentation/screens/role_form_screen.dart';
import 'package:mobile/features/roles/presentation/screens/roles_screen.dart';
import 'package:mobile/features/users/presentation/screens/user_detail_screen.dart';
import 'package:mobile/features/users/presentation/screens/user_form_screen.dart';
import 'package:mobile/features/users/presentation/screens/users_screen.dart';
import 'package:mobile/shared/widgets/main_shell.dart';

final routerProvider = Provider<GoRouter>((ref) {
  final authNotifier = ref.watch(authProvider.notifier);

  return GoRouter(
    initialLocation: '/dashboard',
    redirect: (context, state) {
      final authState = ref.read(authProvider);
      final isAuth = authState.isAuthenticated;
      final isInit = authState.status == AuthStatus.initial;
      final isLoading = authState.status == AuthStatus.loading;

      if (isInit || isLoading) return null;

      final path = state.uri.path;
      final onAuthPage = path == '/login' || path == '/register';

      if (!isAuth && !onAuthPage) return '/login';
      if (isAuth && onAuthPage) return '/dashboard';

      return null;
    },
    refreshListenable: _AuthStateListenable(ref),
    routes: [
      // ── Auth (no shell) ─────────────────────────────────────────────────
      GoRoute(path: '/login', builder: (_, __) => const LoginScreen()),
      GoRoute(path: '/register', builder: (_, __) => const RegisterScreen()),

      // ── Main shell with bottom nav ───────────────────────────────────────
      ShellRoute(
        builder: (_, state, child) => MainShell(child: child),
        routes: [
          GoRoute(
            path: '/dashboard',
            builder: (_, __) => const DashboardScreen(),
          ),
          GoRoute(
            path: '/users',
            builder: (_, __) => const UsersScreen(),
          ),
          GoRoute(
            path: '/roles',
            builder: (_, __) => const RolesScreen(),
          ),
        ],
      ),

      // ── User routes (full screen, no shell) ─────────────────────────────
      GoRoute(
        path: '/users/create',
        builder: (_, __) => const UserFormScreen(),
      ),
      GoRoute(
        path: '/users/:id',
        builder: (_, state) =>
            UserDetailScreen(userId: state.pathParameters['id']!),
      ),
      GoRoute(
        path: '/users/:id/edit',
        builder: (_, state) =>
            UserFormScreen(userId: state.pathParameters['id']),
      ),

      // ── Role routes ──────────────────────────────────────────────────────
      GoRoute(
        path: '/roles/create',
        builder: (_, __) => const RoleFormScreen(),
      ),
      GoRoute(
        path: '/roles/:id/edit',
        builder: (_, state) =>
            RoleFormScreen(roleId: state.pathParameters['id']),
      ),

      // ── Profile ──────────────────────────────────────────────────────────
      GoRoute(
        path: '/profile',
        builder: (_, __) => const ProfileScreen(),
      ),
    ],
    errorBuilder: (_, state) => Scaffold(
      backgroundColor: const Color(0xFF0A0C10),
      body: Center(
        child: Text(
          'Page not found: ${state.uri}',
          style: const TextStyle(color: Colors.white70, fontFamily: 'Sora'),
        ),
      ),
    ),
  );
});

// Listens to auth state changes and refreshes the router
class _AuthStateListenable extends ChangeNotifier {
  _AuthStateListenable(ref) {
    ref.listen(authProvider, (_, __) => notifyListeners());
  }
}
