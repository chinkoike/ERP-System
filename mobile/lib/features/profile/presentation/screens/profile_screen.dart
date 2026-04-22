import 'package:flutter/material.dart';
import 'package:flutter_animate/flutter_animate.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../shared/widgets/user_avatar.dart';
import '../../../../shared/widgets/role_badge.dart';
import '../../../../shared/widgets/info_row.dart';
import '../../../../shared/widgets/app_button.dart';
import '../../../auth/presentation/providers/auth_provider.dart';

class ProfileScreen extends ConsumerWidget {
  const ProfileScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final auth = ref.watch(authProvider);
    final user = auth.user;

    return Scaffold(
      backgroundColor: AppColors.bg0,
      appBar: AppBar(
        title: const Text('Profile'),
        leading: IconButton(
          icon: const Icon(Icons.arrow_back_ios_new_rounded, size: 18),
          onPressed: () => context.pop(),
        ),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Column(
          children: [
            // Avatar card
            Container(
              width: double.infinity,
              padding: const EdgeInsets.all(28),
              decoration: BoxDecoration(
                color: AppColors.bg2,
                borderRadius: BorderRadius.circular(16),
                border: Border.all(color: AppColors.border),
              ),
              child: Column(
                children: [
                  UserAvatar(name: user?.fullName ?? 'User', size: 80),
                  const SizedBox(height: 14),
                  Text(
                    user?.fullName ?? '—',
                    style: const TextStyle(
                      fontFamily: 'Sora',
                      fontWeight: FontWeight.w700,
                      fontSize: 20,
                      color: AppColors.textPrimary,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    '@${user?.username ?? '—'}',
                    style: const TextStyle(
                      fontFamily: 'Sora',
                      fontSize: 13,
                      color: AppColors.textSecondary,
                    ),
                  ),
                  if (user != null && user.roles.isNotEmpty) ...[
                    const SizedBox(height: 14),
                    Wrap(
                      spacing: 8,
                      children: user.roles.map((r) => RoleBadge(role: r)).toList(),
                    ),
                  ],
                ],
              ),
            ).animate().fadeIn(duration: 300.ms).slideY(begin: 0.05, end: 0),

            const SizedBox(height: 16),

            // Info card
            Container(
              width: double.infinity,
              decoration: BoxDecoration(
                color: AppColors.bg2,
                borderRadius: BorderRadius.circular(16),
                border: Border.all(color: AppColors.border),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Padding(
                    padding: EdgeInsets.fromLTRB(16, 16, 16, 8),
                    child: Text(
                      'ACCOUNT DETAILS',
                      style: TextStyle(
                        fontFamily: 'Sora',
                        fontSize: 11,
                        fontWeight: FontWeight.w600,
                        color: AppColors.textMuted,
                        letterSpacing: 1.1,
                      ),
                    ),
                  ),
                  InfoRow(
                    icon: Icons.email_outlined,
                    label: 'Email',
                    value: user?.email ?? '—',
                  ),
                  const Divider(height: 1, indent: 16),
                  InfoRow(
                    icon: Icons.badge_outlined,
                    label: 'First Name',
                    value: user?.firstName ?? '—',
                  ),
                  const Divider(height: 1, indent: 16),
                  InfoRow(
                    icon: Icons.badge_outlined,
                    label: 'Last Name',
                    value: user?.lastName ?? '—',
                  ),
                  const Divider(height: 1, indent: 16),
                  InfoRow(
                    icon: Icons.circle_outlined,
                    label: 'Status',
                    value: user?.isActive == true ? 'Active' : 'Inactive',
                  ),
                ],
              ),
            ).animate().fadeIn(delay: 80.ms, duration: 300.ms),

            const SizedBox(height: 24),

            // Logout button
            AppButton(
              label: 'Sign Out',
              variant: AppButtonVariant.danger,
              icon: Icons.logout_rounded,
              onPressed: () async {
                await ref.read(authProvider.notifier).logout();
                if (context.mounted) context.go('/login');
              },
            ).animate().fadeIn(delay: 160.ms, duration: 300.ms),

            const SizedBox(height: 16),

            // App version label
            const Text(
              'ERP Mobile v1.0.0',
              style: TextStyle(
                fontFamily: 'JetBrainsMono',
                fontSize: 11,
                color: AppColors.textMuted,
              ),
            ).animate().fadeIn(delay: 200.ms),
            const SizedBox(height: 32),
          ],
        ),
      ),
    );
  }
}
