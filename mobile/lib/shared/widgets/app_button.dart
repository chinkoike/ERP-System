import 'package:flutter/material.dart';
import '../../core/theme/app_theme.dart';

enum AppButtonVariant { primary, danger, outline }

class AppButton extends StatelessWidget {
  final String label;
  final VoidCallback? onPressed;
  final bool isLoading;
  final AppButtonVariant variant;
  final IconData? icon;
  final double? width;

  const AppButton({
    super.key,
    required this.label,
    this.onPressed,
    this.isLoading = false,
    this.variant = AppButtonVariant.primary,
    this.icon,
    this.width,
  });

  @override
  Widget build(BuildContext context) {
    final bgColor = switch (variant) {
      AppButtonVariant.primary => AppColors.accent,
      AppButtonVariant.danger => AppColors.danger,
      AppButtonVariant.outline => Colors.transparent,
    };
    final fgColor = switch (variant) {
      AppButtonVariant.outline => AppColors.accent,
      _ => Colors.white,
    };

    return SizedBox(
      width: width ?? double.infinity,
      height: 50,
      child: ElevatedButton(
        onPressed: isLoading ? null : onPressed,
        style: ElevatedButton.styleFrom(
          backgroundColor: bgColor,
          foregroundColor: fgColor,
          disabledBackgroundColor: AppColors.bg3,
          side: variant == AppButtonVariant.outline
              ? const BorderSide(color: AppColors.accent)
              : null,
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
        ),
        child: isLoading
            ? const SizedBox(
                width: 20,
                height: 20,
                child: CircularProgressIndicator(
                  color: Colors.white,
                  strokeWidth: 2,
                ),
              )
            : Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  if (icon != null) ...[
                    Icon(icon, size: 18),
                    const SizedBox(width: 8),
                  ],
                  Text(label),
                ],
              ),
      ),
    );
  }
}
