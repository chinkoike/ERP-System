import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/dio_client.dart';
import '../../../auth/data/models/auth_models.dart';

final usersRemoteDataSourceProvider = Provider<UsersRemoteDataSource>((ref) {
  return UsersRemoteDataSource(ref.read(dioProvider));
});

class UsersRemoteDataSource {
  final Dio _dio;
  UsersRemoteDataSource(this._dio);

  Future<List<UserModel>> getAllUsers() async {
    try {
      final response = await _dio.get(ApiConstants.users);
      return (response.data as List).map((e) => UserModel.fromJson(e)).toList();
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<List<UserModel>> getActiveUsers() async {
    try {
      final response = await _dio.get(ApiConstants.usersActive);
      return (response.data as List).map((e) => UserModel.fromJson(e)).toList();
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<UserModel> getUserById(String id) async {
    try {
      final response = await _dio.get('${ApiConstants.users}/$id');
      return UserModel.fromJson(response.data);
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<PagedResultModel<UserModel>> searchUsers({
    String? query,
    bool? isActive,
    int page = 1,
    int pageSize = 20,
  }) async {
    try {
      final response = await _dio.get(
        ApiConstants.usersSearch,
        queryParameters: {
          if (query != null) 'query': query,
          if (isActive != null) 'isActive': isActive,
          'page': page,
          'pageSize': pageSize,
        },
      );
      return PagedResultModel.fromJson(
        response.data,
        (json) => UserModel.fromJson(json as Map<String, dynamic>),
      );
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<String> createUser(Map<String, dynamic> data) async {
    try {
      final response = await _dio.post(ApiConstants.users, data: data);
      return response.data['id'] as String;
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<void> updateUser(String id, Map<String, dynamic> data) async {
    try {
      await _dio.put('${ApiConstants.users}/$id', data: data);
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<void> deleteUser(String id) async {
    try {
      await _dio.delete('${ApiConstants.users}/$id');
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<bool> existsByUsername(String username) async {
    try {
      final response = await _dio.get('${ApiConstants.users}/exists/username/$username');
      return response.data['exists'] as bool;
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<bool> existsByEmail(String email) async {
    try {
      final response = await _dio.get('${ApiConstants.users}/exists/email/$email');
      return response.data['exists'] as bool;
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }
}
