import 'package:flutter/material.dart';
import 'package:flutter_animate/flutter_animate.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../shared/widgets/role_badge.dart';
import '../../../../shared/widgets/app_snackbar.dart';
import '../../../../shared/widgets/common_widgets.dart';
import '../../../auth/data/models/auth_models.dart';
import '../providers/roles_provider.dart';

class RolesScreen extends ConsumerWidget {
  const RolesScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final rolesAsync = ref.watch(rolesListProvider);

    return Scaffold(
      backgroundColor: AppColors.bg0,
      appBar: AppBar(
        title: const Text('Roles'),
        actions: [
          IconButton(
            icon: const Icon(Icons.add_rounded, size: 22),
            onPressed: () async {
              await context.push('/roles/create');
              ref.invalidate(rolesListProvider);
            },
            tooltip: 'Create Role',
          ),
        ],
      ),
      body: rolesAsync.when(
        loading: () => const Center(
            child: CircularProgressIndicator(color: AppColors.accent)),
        error: (e, _) => ErrorState(
          message: e.toString(),
          onRetry: () => ref.invalidate(rolesListProvider),
        ),
        data: (roles) {
          if (roles.isEmpty) {
            return EmptyState(
              icon: Icons.shield_outlined,
              title: 'No roles yet',
              subtitle: 'Create your first role to get started',
              action: ElevatedButton.icon(
                onPressed: () => context.push('/roles/create'),
                icon: const Icon(Icons.add_rounded, size: 16),
                label: const Text('Create Role',
                    style: TextStyle(fontFamily: 'Sora')),
              ),
            );
          }
          return ListView.separated(
            padding: const EdgeInsets.fromLTRB(16, 12, 16, 80),
            itemCount: roles.length,
            separatorBuilder: (_, __) => const SizedBox(height: 8),
            itemBuilder: (context, i) {
              return _RoleCard(role: roles[i], ref: ref)
                  .animate()
                  .fadeIn(delay: (i * 40).ms, duration: 300.ms)
                  .slideY(begin: 0.05, end: 0);
            },
          );
        },
      ),
    );
  }
}

class _RoleCard extends StatelessWidget {
  final RoleModel role;
  final WidgetRef ref;

  const _RoleCard({required this.role, required this.ref});

  Future<void> _confirmDelete(BuildContext context) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        backgroundColor: AppColors.bg2,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
        title: const Text('Delete Role',
            style: TextStyle(fontFamily: 'Sora', color: AppColors.textPrimary)),
        content: Text(
          'Are you sure you want to delete "${role.name}"?',
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
      final ok = await ref.read(roleFormProvider.notifier).delete(role.id);
      if (context.mounted) {
        if (ok) {
          ref.invalidate(rolesListProvider);
          AppSnackbar.success(context, 'Role deleted');
        } else {
          AppSnackbar.error(context, 'Failed to delete role');
        }
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppColors.bg2,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: AppColors.border),
      ),
      child: Row(
        children: [
          Container(
            width: 44,
            height: 44,
            decoration: BoxDecoration(
              color: AppColors.accentSoft,
              borderRadius: BorderRadius.circular(10),
            ),
            child: const Icon(Icons.shield_outlined,
                color: AppColors.accent, size: 20),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                RoleBadge(role: role.name),
                if (role.description != null && role.description!.isNotEmpty) ...[
                  const SizedBox(height: 4),
                  Text(
                    role.description!,
                    style: const TextStyle(
                      fontFamily: 'Sora',
                      fontSize: 12,
                      color: AppColors.textSecondary,
                    ),
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                  ),
                ],
              ],
            ),
          ),
          PopupMenuButton<String>(
            color: AppColors.bg3,
            shape:
                RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
            icon: const Icon(Icons.more_vert_rounded,
                color: AppColors.textSecondary, size: 20),
            onSelected: (v) async {
              if (v == 'edit') {
                await context.push('/roles/${role.id}/edit');
                ref.invalidate(rolesListProvider);
              } else if (v == 'delete') {
                await _confirmDelete(context);
              }
            },
            itemBuilder: (_) => [
              const PopupMenuItem(
                value: 'edit',
                child: Row(children: [
                  Icon(Icons.edit_outlined, size: 16, color: AppColors.textPrimary),
                  SizedBox(width: 10),
                  Text('Edit', style: TextStyle(fontFamily: 'Sora', color: AppColors.textPrimary)),
                ]),
              ),
              const PopupMenuItem(
                value: 'delete',
                child: Row(children: [
                  Icon(Icons.delete_outline_rounded, size: 16, color: AppColors.danger),
                  SizedBox(width: 10),
                  Text('Delete', style: TextStyle(fontFamily: 'Sora', color: AppColors.danger)),
                ]),
              ),
            ],
          ),
        ],
      ),
    );
  }
}
