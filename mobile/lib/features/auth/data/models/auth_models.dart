import 'package:json_annotation/json_annotation.dart';

part 'auth_models.g.dart';

@JsonSerializable()
class LoginRequestModel {
  final String username;
  final String password;

  const LoginRequestModel({required this.username, required this.password});

  factory LoginRequestModel.fromJson(Map<String, dynamic> json) =>
      _$LoginRequestModelFromJson(json);
  Map<String, dynamic> toJson() => _$LoginRequestModelToJson(this);
}

@JsonSerializable()
class RegisterRequestModel {
  final String username;
  final String email;
  final String password;
  final String? firstName;
  final String? lastName;

  const RegisterRequestModel({
    required this.username,
    required this.email,
    required this.password,
    this.firstName,
    this.lastName,
  });

  factory RegisterRequestModel.fromJson(Map<String, dynamic> json) =>
      _$RegisterRequestModelFromJson(json);
  Map<String, dynamic> toJson() => _$RegisterRequestModelToJson(this);
}

@JsonSerializable()
class AuthResponseModel {
  @JsonKey(name: 'token')
  final String? accessToken;
  final String? refreshToken;
  final bool isSuccess;
  final String? message;
  final UserModel? user;
  final List<String>? roles;

  const AuthResponseModel({
    this.accessToken,
    this.refreshToken,
    required this.isSuccess,
    this.message,
    this.user,
    this.roles,
  });

  factory AuthResponseModel.fromJson(Map<String, dynamic> json) =>
      _$AuthResponseModelFromJson(json);
  Map<String, dynamic> toJson() => _$AuthResponseModelToJson(this);
}

@JsonSerializable()
class UserModel {
  final String id;
  final String username;
  final String email;
  final String? firstName;
  final String? lastName;
  final String? fullName;
  final String? jobTitle;
  final String? department;
  final bool isActive;
  final List<String> roles;
  final DateTime? createdAt;
  final DateTime? lastLoginAt;
  final String? createdBy;
  final DateTime? updatedAt;
  final String? updatedBy;

  const UserModel({
    required this.id,
    required this.username,
    required this.email,
    this.firstName,
    this.lastName,
    this.fullName,
    this.jobTitle,
    this.department,
    required this.isActive,
    required this.roles,
    this.createdAt,
    this.lastLoginAt,
    this.createdBy,
    this.updatedAt,
    this.updatedBy,
  });

  String get displayName {
    if (fullName != null && fullName!.isNotEmpty) return fullName!;
    if (firstName != null && lastName != null) return '$firstName $lastName';
    if (firstName != null) return firstName!;
    return username;
  }

  String get initials {
    final name = displayName;
    final parts = name.trim().split(' ');
    if (parts.length >= 2) return '${parts[0][0]}${parts[1][0]}'.toUpperCase();
    return name.isNotEmpty ? name[0].toUpperCase() : '?';
  }

  factory UserModel.fromJson(Map<String, dynamic> json) =>
      _$UserModelFromJson(json);
  Map<String, dynamic> toJson() => _$UserModelToJson(this);
}

@JsonSerializable()
class RoleModel {
  final String id;
  final String name;
  final String? description;
  final DateTime? createdAt;

  const RoleModel({
    required this.id,
    required this.name,
    this.description,
    this.createdAt,
  });

  factory RoleModel.fromJson(Map<String, dynamic> json) =>
      _$RoleModelFromJson(json);
  Map<String, dynamic> toJson() => _$RoleModelToJson(this);
}

@JsonSerializable(genericArgumentFactories: true)
class PagedResultModel<T> {
  final List<T> items;
  final int totalCount;

  // API ส่ง "pageNumber" ไม่ใช่ "page"
  @JsonKey(name: 'pageNumber')
  final int page;

  final int pageSize;
  final int totalPages;

  const PagedResultModel({
    required this.items,
    required this.totalCount,
    required this.page,
    required this.pageSize,
    required this.totalPages,
  });

  bool get hasNext => page < totalPages;
  bool get hasPrev => page > 1;

  factory PagedResultModel.fromJson(
    Map<String, dynamic> json,
    T Function(Object? json) fromJsonT,
  ) =>
      _$PagedResultModelFromJson(json, fromJsonT);

  Map<String, dynamic> toJson(Object? Function(T value) toJsonT) =>
      _$PagedResultModelToJson(this, toJsonT);
}
