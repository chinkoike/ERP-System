import 'package:flutter/material.dart';
import 'package:flutter_animate/flutter_animate.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../shared/widgets/user_avatar.dart';
import '../../../../shared/widgets/role_badge.dart';
import '../../../../shared/widgets/empty_state.dart';
import '../../../../shared/widgets/error_state.dart';
import '../providers/users_provider.dart';
import '../../../auth/data/models/auth_models.dart';

class UsersScreen extends ConsumerStatefulWidget {
  const UsersScreen({super.key});

  @override
  ConsumerState<UsersScreen> createState() => _UsersScreenState();
}

class _UsersScreenState extends ConsumerState<UsersScreen> {
  final _searchCtrl = TextEditingController();

  @override
  void dispose() {
    _searchCtrl.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final searchState = ref.watch(userSearchProvider);

    return Scaffold(
      backgroundColor: AppColors.bg0,
      appBar: AppBar(
        title: const Text('Users'),
        actions: [
          IconButton(
            icon: const Icon(Icons.person_add_outlined, size: 22),
            onPressed: () => context.push('/users/create'),
            tooltip: 'Create User',
          ),
        ],
      ),
      body: Column(
        children: [
          // Search bar
          Padding(
            padding: const EdgeInsets.fromLTRB(16, 8, 16, 0),
            child: _SearchBar(
              controller: _searchCtrl,
              onChanged: (q) =>
                  ref.read(userSearchProvider.notifier).search(query: q),
            ),
          ).animate().fadeIn(duration: 300.ms),

          // Filter chips
          _FilterRow(
            currentFilter: searchState.filterActive,
            onFilter: (active) =>
                ref.read(userSearchProvider.notifier).search(isActive: active),
          ).animate().fadeIn(delay: 50.ms),

          // Results
          Expanded(
            child: _buildBody(context, searchState),
          ),
        ],
      ),
    );
  }

  Widget _buildBody(BuildContext context, UserSearchState state) {
    if (state.isLoading && state.result == null) {
      return const Center(
          child: CircularProgressIndicator(color: AppColors.accent));
    }
    if (state.error != null && state.result == null) {
      return ErrorState(
        message: state.error!,
        onRetry: () => ref.read(userSearchProvider.notifier).search(),
      );
    }
    final users = state.result?.items ?? [];
    if (users.isEmpty) {
      return const EmptyState(
        icon: Icons.people_outline,
        title: 'No users found',
        subtitle: 'Try a different search term',
      );
    }
    return NotificationListener<ScrollNotification>(
      onNotification: (notif) {
        if (notif.metrics.pixels >= notif.metrics.maxScrollExtent - 200) {
          ref.read(userSearchProvider.notifier).nextPage();
        }
        return false;
      },
      child: ListView.separated(
        padding: const EdgeInsets.fromLTRB(16, 12, 16, 80),
        itemCount: users.length + (state.isLoading ? 1 : 0),
        separatorBuilder: (_, __) => const SizedBox(height: 8),
        itemBuilder: (context, i) {
          if (i >= users.length) {
            return const Padding(
              padding: EdgeInsets.all(16),
              child: Center(
                  child: CircularProgressIndicator(color: AppColors.accent)),
            );
          }
          return _UserCard(user: users[i])
              .animate()
              .fadeIn(delay: (i * 30).ms, duration: 300.ms)
              .slideY(begin: 0.05, end: 0);
        },
      ),
    );
  }
}

class _SearchBar extends StatelessWidget {
  final TextEditingController controller;
  final ValueChanged<String> onChanged;

  const _SearchBar({required this.controller, required this.onChanged});

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        color: AppColors.bg2,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.border),
      ),
      child: TextField(
        controller: controller,
        onChanged: onChanged,
        style:
            const TextStyle(color: AppColors.textPrimary, fontFamily: 'Sora'),
        decoration: const InputDecoration(
          hintText: 'Search by name, username, email…',
          prefixIcon: Icon(Icons.search_rounded,
              color: AppColors.textSecondary, size: 20),
          border: InputBorder.none,
          enabledBorder: InputBorder.none,
          focusedBorder: InputBorder.none,
          contentPadding: EdgeInsets.symmetric(vertical: 14),
        ),
      ),
    );
  }
}

class _FilterRow extends StatelessWidget {
  final bool? currentFilter;
  final ValueChanged<bool?> onFilter;

  const _FilterRow({required this.currentFilter, required this.onFilter});

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
      child: Row(
        children: [
          _Chip(
              label: 'All',
              selected: currentFilter == null,
              onTap: () => onFilter(null)),
          const SizedBox(width: 8),
          _Chip(
              label: 'Active',
              selected: currentFilter == true,
              onTap: () => onFilter(true)),
          const SizedBox(width: 8),
          _Chip(
              label: 'Inactive',
              selected: currentFilter == false,
              onTap: () => onFilter(false)),
        ],
      ),
    );
  }
}

class _Chip extends StatelessWidget {
  final String label;
  final bool selected;
  final VoidCallback onTap;

  const _Chip(
      {required this.label, required this.selected, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: AnimatedContainer(
        duration: 200.ms,
        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 7),
        decoration: BoxDecoration(
          color: selected ? AppColors.accentSoft : AppColors.bg2,
          borderRadius: BorderRadius.circular(20),
          border: Border.all(
            color: selected ? AppColors.accent : AppColors.border,
          ),
        ),
        child: Text(
          label,
          style: TextStyle(
            fontFamily: 'Sora',
            fontSize: 13,
            fontWeight: selected ? FontWeight.w600 : FontWeight.w400,
            color: selected ? AppColors.accent : AppColors.textSecondary,
          ),
        ),
      ),
    );
  }
}

class _UserCard extends ConsumerWidget {
  final UserModel user;

  const _UserCard({required this.user});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    return GestureDetector(
      onTap: () => context.push('/users/${user.id}'),
      child: Container(
        padding: const EdgeInsets.all(14),
        decoration: BoxDecoration(
          color: AppColors.bg2,
          borderRadius: BorderRadius.circular(14),
          border: Border.all(color: AppColors.border),
        ),
        child: Row(
          children: [
            UserAvatar(name: user.fullName, size: 44),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      Expanded(
                        child: Text(
                          user.fullName,
                          style: const TextStyle(
                            fontFamily: 'Sora',
                            fontWeight: FontWeight.w600,
                            fontSize: 14,
                            color: AppColors.textPrimary,
                          ),
                          overflow: TextOverflow.ellipsis,
                        ),
                      ),
                      Container(
                        width: 8,
                        height: 8,
                        decoration: BoxDecoration(
                          color: user.isActive
                              ? AppColors.success
                              : AppColors.textMuted,
                          shape: BoxShape.circle,
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 3),
                  Text(
                    '@${user.username} · ${user.email}',
                    style: const TextStyle(
                      fontFamily: 'Sora',
                      fontSize: 12,
                      color: AppColors.textSecondary,
                    ),
                    overflow: TextOverflow.ellipsis,
                  ),
                  if (user.roles.isNotEmpty) ...[
                    const SizedBox(height: 8),
                    Wrap(
                      spacing: 6,
                      runSpacing: 4,
                      children:
                          user.roles.map((r) => RoleBadge(role: r)).toList(),
                    ),
                  ],
                ],
              ),
            ),
            const SizedBox(width: 8),
            const Icon(Icons.chevron_right_rounded,
                color: AppColors.textMuted, size: 20),
          ],
        ),
      ),
    );
  }
}
