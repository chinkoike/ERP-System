import 'package:flutter/material.dart';

class AppColors {
  // Background layers
  static const Color bg0 = Color(0xFF0A0C10); // deepest
  static const Color bg1 = Color(0xFF111318); // surface
  static const Color bg2 = Color(0xFF1A1D24); // card
  static const Color bg3 = Color(0xFF22262F); // elevated card

  // Brand / Accent
  static const Color accent = Color(0xFF4F8EF7); // primary blue
  static const Color accentSoft = Color(0xFF1E3A6E); // muted blue fill
  static const Color accentGlow = Color(0x334F8EF7); // glow layer

  static const Color success = Color(0xFF34D399);
  static const Color successSoft = Color(0x2034D399);
  static const Color warning = Color(0xFFFBBF24);
  static const Color warningSoft = Color(0x20FBBF24);
  static const Color danger = Color(0xFFF87171);
  static const Color dangerSoft = Color(0x20F87171);

  // Text
  static const Color textPrimary = Color(0xFFEDF0F7);
  static const Color textSecondary = Color(0xFF8892A4);
  static const Color textMuted = Color(0xFF4A5568);

  // Border
  static const Color border = Color(0xFF252A36);
  static const Color borderBright = Color(0xFF353D4F);

  // Role badge colors
  static const Color roleAdmin = Color(0xFFF87171);
  static const Color roleManager = Color(0xFFFBBF24);
  static const Color roleUser = Color(0xFF4F8EF7);
}

class AppTheme {
  static ThemeData get dark {
    return ThemeData(
      brightness: Brightness.dark,
      scaffoldBackgroundColor: AppColors.bg0,
      fontFamily: 'Sora',
      colorScheme: const ColorScheme.dark(
        primary: AppColors.accent,
        surface: AppColors.bg1,
        onSurface: AppColors.textPrimary,
        error: AppColors.danger,
      ),
      appBarTheme: const AppBarTheme(
        backgroundColor: AppColors.bg0,
        elevation: 0,
        centerTitle: false,
        iconTheme: IconThemeData(color: AppColors.textPrimary),
        titleTextStyle: TextStyle(
          fontFamily: 'Sora',
          fontSize: 18,
          fontWeight: FontWeight.w600,
          color: AppColors.textPrimary,
        ),
      ),
      cardTheme: CardThemeData(
        color: AppColors.bg2,
        elevation: 0,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(14),
          side: const BorderSide(color: AppColors.border, width: 1),
        ),
      ),
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: AppColors.bg2,
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(10),
          borderSide: const BorderSide(color: AppColors.border),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(10),
          borderSide: const BorderSide(color: AppColors.border),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(10),
          borderSide: const BorderSide(color: AppColors.accent, width: 1.5),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(10),
          borderSide: const BorderSide(color: AppColors.danger),
        ),
        labelStyle:
            const TextStyle(color: AppColors.textSecondary, fontFamily: 'Sora'),
        hintStyle:
            const TextStyle(color: AppColors.textMuted, fontFamily: 'Sora'),
        prefixIconColor: AppColors.textSecondary,
      ),
      elevatedButtonTheme: ElevatedButtonThemeData(
        style: ElevatedButton.styleFrom(
          backgroundColor: AppColors.accent,
          foregroundColor: Colors.white,
          elevation: 0,
          shape:
              RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
          textStyle: const TextStyle(
            fontFamily: 'Sora',
            fontWeight: FontWeight.w600,
            fontSize: 14,
          ),
          padding: const EdgeInsets.symmetric(vertical: 14, horizontal: 24),
        ),
      ),
      textButtonTheme: TextButtonThemeData(
        style: TextButton.styleFrom(
          foregroundColor: AppColors.accent,
          textStyle:
              const TextStyle(fontFamily: 'Sora', fontWeight: FontWeight.w500),
        ),
      ),
      dividerTheme: const DividerThemeData(
        color: AppColors.border,
        thickness: 1,
      ),
      bottomNavigationBarTheme: const BottomNavigationBarThemeData(
        backgroundColor: AppColors.bg1,
        selectedItemColor: AppColors.accent,
        unselectedItemColor: AppColors.textMuted,
        type: BottomNavigationBarType.fixed,
        elevation: 0,
      ),
      textTheme: const TextTheme(
        displayLarge: TextStyle(
            fontFamily: 'Sora',
            fontWeight: FontWeight.w700,
            color: AppColors.textPrimary),
        displayMedium: TextStyle(
            fontFamily: 'Sora',
            fontWeight: FontWeight.w700,
            color: AppColors.textPrimary),
        headlineLarge: TextStyle(
            fontFamily: 'Sora',
            fontWeight: FontWeight.w600,
            color: AppColors.textPrimary),
        headlineMedium: TextStyle(
            fontFamily: 'Sora',
            fontWeight: FontWeight.w600,
            color: AppColors.textPrimary),
        titleLarge: TextStyle(
            fontFamily: 'Sora',
            fontWeight: FontWeight.w600,
            color: AppColors.textPrimary),
        titleMedium: TextStyle(
            fontFamily: 'Sora',
            fontWeight: FontWeight.w500,
            color: AppColors.textPrimary),
        bodyLarge: TextStyle(fontFamily: 'Sora', color: AppColors.textPrimary),
        bodyMedium:
            TextStyle(fontFamily: 'Sora', color: AppColors.textSecondary),
        labelLarge: TextStyle(
            fontFamily: 'Sora',
            fontWeight: FontWeight.w600,
            color: AppColors.textPrimary),
      ),
    );
  }
}
