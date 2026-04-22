// ─── app_snackbar.dart ────────────────────────────────────────────────────────
// Usage: AppSnackbar.success(context, 'Done!');

import 'package:flutter/material.dart';
import '../../core/theme/app_theme.dart';

class AppSnackbar {
  static void success(BuildContext context, String message) {
    _show(context, message, AppColors.success, Icons.check_circle_outline_rounded);
  }

  static void error(BuildContext context, String message) {
    _show(context, message, AppColors.danger, Icons.error_outline_rounded);
  }

  static void info(BuildContext context, String message) {
    _show(context, message, AppColors.accent, Icons.info_outline_rounded);
  }

  static void _show(BuildContext context, String message, Color color, IconData icon) {
    ScaffoldMessenger.of(context).clearSnackBars();
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        behavior: SnackBarBehavior.floating,
        backgroundColor: AppColors.bg3,
        margin: const EdgeInsets.all(16),
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(12),
          side: BorderSide(color: color.withOpacity(0.4)),
        ),
        content: Row(
          children: [
            Icon(icon, color: color, size: 18),
            const SizedBox(width: 10),
            Expanded(
              child: Text(
                message,
                style: const TextStyle(
                  fontFamily: 'Sora',
                  color: AppColors.textPrimary,
                  fontSize: 13,
                ),
              ),
            ),
          ],
        ),
        duration: const Duration(seconds: 3),
      ),
    );
  }
}
