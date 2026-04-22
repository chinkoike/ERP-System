// ─── Roles Remote Datasource ──────────────────────────────────────────────────
// lib/features/roles/data/datasources/roles_remote_datasource.dart

import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/dio_client.dart';
import '../../../auth/data/models/auth_models.dart';

final rolesRemoteDataSourceProvider = Provider<RolesRemoteDataSource>((ref) {
  return RolesRemoteDataSource(ref.read(dioProvider));
});

class RolesRemoteDataSource {
  final Dio _dio;
  RolesRemoteDataSource(this._dio);

  Future<List<RoleModel>> getAllRoles() async {
    try {
      final response = await _dio.get(ApiConstants.roles);
      return (response.data as List).map((e) => RoleModel.fromJson(e)).toList();
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<RoleModel> getRoleById(String id) async {
    try {
      final response = await _dio.get('${ApiConstants.roles}/$id');
      return RoleModel.fromJson(response.data);
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<String> createRole(String name, String? description) async {
    try {
      final response = await _dio.post(ApiConstants.roles, data: {
        'name': name,
        if (description != null) 'description': description,
      });
      return response.data['id'] as String;
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<void> updateRole(String id, String name, String? description) async {
    try {
      await _dio.put('${ApiConstants.roles}/$id', data: {
        'name': name,
        if (description != null) 'description': description,
      });
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<void> deleteRole(String id) async {
    try {
      await _dio.delete('${ApiConstants.roles}/$id');
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<bool> existsByName(String name) async {
    try {
      final response =
          await _dio.get('${ApiConstants.roles}/exists/name/$name');
      return response.data['exists'] as bool;
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }
}
