import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/dio_client.dart';
import '../models/auth_models.dart';

final authRemoteDataSourceProvider = Provider<AuthRemoteDataSource>((ref) {
  return AuthRemoteDataSource(ref.read(dioProvider));
});

class AuthRemoteDataSource {
  final Dio _dio;

  AuthRemoteDataSource(this._dio);

  Future<AuthResponseModel> login(LoginRequestModel request) async {
    try {
      final response =
          await _dio.post(ApiConstants.login, data: request.toJson());

      print('[RAW LOGIN RESPONSE] ${response.data}');

      return AuthResponseModel.fromJson(response.data);
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<UserModel> register(RegisterRequestModel request) async {
    try {
      final response =
          await _dio.post(ApiConstants.register, data: request.toJson());
      return UserModel.fromJson(response.data);
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<AuthResponseModel> refreshToken(String token) async {
    try {
      final response = await _dio
          .post(ApiConstants.refreshToken, data: {'refreshToken': token});
      return AuthResponseModel.fromJson(response.data);
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<void> logout(String refreshToken) async {
    try {
      await _dio
          .post(ApiConstants.logout, data: {'refreshToken': refreshToken});
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }
}
