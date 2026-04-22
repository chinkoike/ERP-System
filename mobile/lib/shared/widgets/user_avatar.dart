import 'package:flutter/material.dart';

class UserAvatar extends StatelessWidget {
  final String name;
  final double size;
  final String? imageUrl;

  const UserAvatar({
    super.key,
    required this.name,
    this.size = 40,
    this.imageUrl,
  });

  String get _initials {
    final parts = name.trim().split(' ');
    if (parts.length >= 2) return '${parts[0][0]}${parts[1][0]}'.toUpperCase();
    return name.isNotEmpty ? name[0].toUpperCase() : '?';
  }

  Color _colorFromName(String name) {
    final colors = [
      const Color(0xFF4F8EF7),
      const Color(0xFF34D399),
      const Color(0xFFFBBF24),
      const Color(0xFFF87171),
      const Color(0xFFA78BFA),
      const Color(0xFF38BDF8),
      const Color(0xFFFB923C),
    ];
    int hash = 0;
    for (final c in name.codeUnits) {
      hash = (hash * 31 + c) & 0xFFFFFFFF;
    }
    return colors[hash % colors.length];
  }

  @override
  Widget build(BuildContext context) {
    final color = _colorFromName(name);

    if (imageUrl != null) {
      return CircleAvatar(
        radius: size / 2,
        backgroundImage: NetworkImage(imageUrl!),
        backgroundColor: color.withValues(alpha: 0.2),
      );
    }

    return Container(
      width: size,
      height: size,
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.15),
        shape: BoxShape.circle,
        border: Border.all(color: color.withValues(alpha: 0.3), width: 1.5),
      ),
      child: Center(
        child: Text(
          _initials,
          style: TextStyle(
            fontFamily: 'Sora',
            fontWeight: FontWeight.w700,
            fontSize: size * 0.33,
            color: color,
          ),
        ),
      ),
    );
  }
}
