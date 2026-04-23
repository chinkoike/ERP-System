import 'package:flutter/material.dart';
import 'package:flutter_animate/flutter_animate.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../shared/widgets/user_avatar.dart';
import '../../../../shared/widgets/role_badge.dart';
import '../../../../shared/widgets/app_snackbar.dart';
import '../../../../shared/widgets/info_row.dart';
import '../../data/repositories/users_repository.dart';
import '../providers/users_provider.dart';
import '../../../auth/data/models/auth_models.dart';

final _userDetailProvider = FutureProvider.family<UserModel, String>((ref, id) {
  return ref.read(usersRepositoryProvider).getUserById(id);
});

class UserDetailScreen extends ConsumerWidget {
  final String userId;

  const UserDetailScreen({super.key, required this.userId});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final userAsync = ref.watch(_userDetailProvider(userId));

    return Scaffold(
      backgroundColor: AppColors.bg0,
      appBar: AppBar(
        title: const Text('User Detail'),
        leading: IconButton(
          icon: const Icon(Icons.arrow_back_ios_new_rounded, size: 18),
          onPressed: () => context.pop(),
        ),
        actions: [
          IconButton(
            icon: const Icon(Icons.edit_outlined, size: 20),
            onPressed: () => context.push('/users/$userId/edit'),
          ),
        ],
      ),
      body: userAsync.when(
        loading: () => const Center(
            child: CircularProgressIndicator(color: AppColors.accent)),
        error: (e, _) => Center(
            child: Text(e.toString(),
                style: const TextStyle(color: AppColors.danger))),
        data: (user) => _UserDetailBody(user: user, ref: ref),
      ),
    );
  }
}

class _UserDetailBody extends ConsumerWidget {
  final UserModel user;
  final WidgetRef ref;

  const _UserDetailBody({required this.user, required this.ref});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        children: [
          // Avatar header card
          Container(
            width: double.infinity,
            padding: const EdgeInsets.all(24),
            decoration: BoxDecoration(
              color: AppColors.bg2,
              borderRadius: BorderRadius.circular(16),
              border: Border.all(color: AppColors.border),
            ),
            child: Column(
              children: [
                UserAvatar(name: user.fullName!, size: 72),
                const SizedBox(height: 14),
                Text(
                  user.fullName!,
                  style: const TextStyle(
                    fontFamily: 'Sora',
                    fontWeight: FontWeight.w700,
                    fontSize: 20,
                    color: AppColors.textPrimary,
                  ),
                ),
                const SizedBox(height: 4),
                Text(
                  '@${user.username}',
                  style: const TextStyle(
                    fontFamily: 'Sora',
                    color: AppColors.textSecondary,
                    fontSize: 14,
                  ),
                ),
                const SizedBox(height: 12),
                Row(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    _statusChip(user.isActive),
                  ],
                ),
                if (user.roles.isNotEmpty) ...[
                  const SizedBox(height: 12),
                  Wrap(
                    spacing: 8,
                    children:
                        user.roles.map((r) => RoleBadge(role: r)).toList(),
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
                    'Account Information',
                    style: TextStyle(
                      fontFamily: 'Sora',
                      fontWeight: FontWeight.w600,
                      fontSize: 13,
                      color: AppColors.textSecondary,
                      letterSpacing: 0.5,
                    ),
                  ),
                ),
                InfoRow(
                    icon: Icons.email_outlined,
                    label: 'Email',
                    value: user.email),
                const Divider(height: 1, indent: 16),
                InfoRow(
                    icon: Icons.badge_outlined,
                    label: 'First Name',
                    value: user.firstName ?? '—'),
                const Divider(height: 1, indent: 16),
                InfoRow(
                    icon: Icons.badge_outlined,
                    label: 'Last Name',
                    value: user.lastName ?? '—'),
                const Divider(height: 1, indent: 16),
                InfoRow(
                  icon: Icons.calendar_today_outlined,
                  label: 'Created',
                  value: user.createdAt != null
                      ? '${user.createdAt!.day}/${user.createdAt!.month}/${user.createdAt!.year}'
                      : '—',
                ),
              ],
            ),
          ).animate().fadeIn(delay: 100.ms, duration: 300.ms),

          const SizedBox(height: 16),

          // Danger zone
          Container(
            width: double.infinity,
            decoration: BoxDecoration(
              color: AppColors.bg2,
              borderRadius: BorderRadius.circular(16),
              border: Border.all(color: AppColors.dangerSoft),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Padding(
                  padding: EdgeInsets.fromLTRB(16, 16, 16, 8),
                  child: Text(
                    'Danger Zone',
                    style: TextStyle(
                      fontFamily: 'Sora',
                      fontWeight: FontWeight.w600,
                      fontSize: 13,
                      color: AppColors.danger,
                      letterSpacing: 0.5,
                    ),
                  ),
                ),
                ListTile(
                  leading: const Icon(Icons.delete_outline_rounded,
                      color: AppColors.danger, size: 20),
                  title: const Text(
                    'Delete User',
                    style: TextStyle(
                      fontFamily: 'Sora',
                      color: AppColors.danger,
                      fontWeight: FontWeight.w500,
                    ),
                  ),
                  subtitle: const Text(
                    'This action cannot be undone',
                    style: TextStyle(
                      fontFamily: 'Sora',
                      color: AppColors.textMuted,
                      fontSize: 12,
                    ),
                  ),
                  onTap: () => _confirmDelete(context, ref),
                ),
              ],
            ),
          ).animate().fadeIn(delay: 150.ms, duration: 300.ms),
        ],
      ),
    );
  }

  Widget _statusChip(bool active) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 5),
      decoration: BoxDecoration(
        color: active ? AppColors.successSoft : AppColors.dangerSoft,
        borderRadius: BorderRadius.circular(20),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Container(
            width: 6,
            height: 6,
            decoration: BoxDecoration(
              color: active ? AppColors.success : AppColors.danger,
              shape: BoxShape.circle,
            ),
          ),
          const SizedBox(width: 6),
          Text(
            active ? 'Active' : 'Inactive',
            style: TextStyle(
              fontFamily: 'Sora',
              fontSize: 12,
              fontWeight: FontWeight.w600,
              color: active ? AppColors.success : AppColors.danger,
            ),
          ),
        ],
      ),
    );
  }

  Future<void> _confirmDelete(BuildContext context, WidgetRef ref) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        backgroundColor: AppColors.bg2,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        title: const Text('Delete User',
            style: TextStyle(fontFamily: 'Sora', color: AppColors.textPrimary)),
        content: Text(
          'Are you sure you want to delete "${user.fullName}"? This cannot be undone.',
          style: const TextStyle(
              fontFamily: 'Sora', color: AppColors.textSecondary),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx, false),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            style: ElevatedButton.styleFrom(backgroundColor: AppColors.danger),
            onPressed: () => Navigator.pop(ctx, true),
            child: const Text('Delete'),
          ),
        ],
      ),
    );
    if (confirmed == true) {
      final ok = await ref.read(userFormProvider.notifier).delete(user.id);
      if (context.mounted) {
        if (ok) {
          AppSnackbar.success(context, 'User deleted');
          context.pop();
        } else {
          AppSnackbar.error(context, 'Failed to delete user');
        }
      }
    }
  }
}
