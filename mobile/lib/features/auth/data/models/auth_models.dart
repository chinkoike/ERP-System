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
  final String? accessToken;
  final String? refreshToken;
  final bool isSuccess;
  final String? message;
  final UserModel? user;

  const AuthResponseModel({
    this.accessToken,
    this.refreshToken,
    required this.isSuccess,
    this.message,
    this.user,
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
  final bool isActive;
  final List<String> roles;
  final DateTime? createdAt;

  const UserModel({
    required this.id,
    required this.username,
    required this.email,
    this.firstName,
    this.lastName,
    required this.isActive,
    required this.roles,
    this.createdAt,
  });

  String get fullName {
    if (firstName != null && lastName != null) return '$firstName $lastName';
    if (firstName != null) return firstName!;
    return username;
  }

  String get initials {
    final name = fullName;
    final parts = name.split(' ');
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
  final int page;
  final int pageSize;

  const PagedResultModel({
    required this.items,
    required this.totalCount,
    required this.page,
    required this.pageSize,
  });

  int get totalPages => (totalCount / pageSize).ceil();
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
