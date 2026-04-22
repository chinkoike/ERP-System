import 'package:flutter/material.dart';
import '../../core/theme/app_theme.dart';

class RoleBadge extends StatelessWidget {
  final String role;

  const RoleBadge({super.key, required this.role});

  Color _bgColor() {
    switch (role.toLowerCase()) {
      case 'admin':
        return AppColors.dangerSoft;
      case 'manager':
        return AppColors.warningSoft;
      default:
        return AppColors.accentGlow;
    }
  }

  Color _fgColor() {
    switch (role.toLowerCase()) {
      case 'admin':
        return AppColors.roleAdmin;
      case 'manager':
        return AppColors.roleManager;
      default:
        return AppColors.roleUser;
    }
  }

  IconData _icon() {
    switch (role.toLowerCase()) {
      case 'admin':
        return Icons.shield_outlined;
      case 'manager':
        return Icons.manage_accounts_outlined;
      default:
        return Icons.person_outline_rounded;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
      decoration: BoxDecoration(
        color: _bgColor(),
        borderRadius: BorderRadius.circular(6),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(_icon(), size: 11, color: _fgColor()),
          const SizedBox(width: 4),
          Text(
            role,
            style: TextStyle(
              fontFamily: 'Sora',
              fontSize: 11,
              fontWeight: FontWeight.w600,
              color: _fgColor(),
            ),
          ),
        ],
      ),
    );
  }
}
