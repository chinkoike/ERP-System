import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/dio_client.dart';
import '../../../auth/data/models/auth_models.dart';
import '../models/purchasing_models.dart';

final purchasingRemoteDataSourceProvider =
    Provider<PurchasingRemoteDataSource>((ref) {
  return PurchasingRemoteDataSource(ref.read(dioProvider));
});

class PurchasingRemoteDataSource {
  final Dio _dio;
  PurchasingRemoteDataSource(this._dio);

  Future<List<SupplierModel>> getAllSuppliers() async {
    try {
      final response = await _dio.get(ApiConstants.purchasingSuppliers);
      return (response.data as List)
          .map((e) => SupplierModel.fromJson(e as Map<String, dynamic>))
          .toList();
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<SupplierModel> getSupplierById(String id) async {
    try {
      final response = await _dio.get('${ApiConstants.purchasingSuppliers}/$id');
      return SupplierModel.fromJson(response.data as Map<String, dynamic>);
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<String> createSupplier({
    required String name,
    String? contactName,
    String? email,
    String? phone,
    String? address,
  }) async {
    try {
      final response = await _dio.post(
        ApiConstants.purchasingSuppliers,
        data: {
          'name': name,
          if (contactName != null) 'contactName': contactName,
          if (email != null) 'email': email,
          if (phone != null) 'phone': phone,
          if (address != null) 'address': address,
        },
      );
      return response.data['id'] as String;
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<void> updateSupplier(
    String id, {
    String? name,
    String? contactName,
    String? email,
    String? phone,
    String? address,
  }) async {
    try {
      await _dio.put(
        '${ApiConstants.purchasingSuppliers}/$id',
        data: {
          if (name != null) 'name': name,
          if (contactName != null) 'contactName': contactName,
          if (email != null) 'email': email,
          if (phone != null) 'phone': phone,
          if (address != null) 'address': address,
        },
      );
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<void> deleteSupplier(String id) async {
    try {
      await _dio.delete('${ApiConstants.purchasingSuppliers}/$id');
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<List<PurchaseOrderModel>> getAllPurchaseOrders() async {
    try {
      final response = await _dio.get(ApiConstants.purchaseOrders);
      return (response.data as List)
          .map((e) => PurchaseOrderModel.fromJson(e as Map<String, dynamic>))
          .toList();
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<List<PurchaseOrderModel>> searchPurchaseOrders({
    String? supplierId,
    String? status,
    DateTime? startDate,
    DateTime? endDate,
  }) async {
    try {
      final response = await _dio.get(
        ApiConstants.purchaseOrdersSearch,
        queryParameters: {
          if (supplierId != null) 'supplierId': supplierId,
          if (status != null) 'status': status,
          if (startDate != null) 'startDate': startDate.toIso8601String(),
          if (endDate != null) 'endDate': endDate.toIso8601String(),
        },
      );
      final paged = PurchaseOrderSearchResponseModel.fromJson(
        response.data as Map<String, dynamic>,
      );
      return paged.items;
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<PurchaseOrderModel> getPurchaseOrderById(String id) async {
    try {
      final response = await _dio.get('${ApiConstants.purchaseOrders}/$id');
      return PurchaseOrderModel.fromJson(response.data as Map<String, dynamic>);
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<String> createPurchaseOrder({
    required String supplierId,
    required List<PurchaseOrderItemModel> items,
  }) async {
    try {
      final response = await _dio.post(
        ApiConstants.purchaseOrders,
        data: {
          'supplierId': supplierId,
          'items': items.map((e) => e.toJson()).toList(),
        },
      );
      return response.data['id'] as String;
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<void> receivePurchaseOrder(
    String id,
    List<PurchaseOrderItemModel> items,
  ) async {
    try {
      await _dio.post(
        '${ApiConstants.purchaseOrders}/$id/receive',
        data: items.map((e) => e.toJson()).toList(),
      );
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }

  Future<void> cancelPurchaseOrder(String id) async {
    try {
      await _dio.patch('${ApiConstants.purchaseOrders}/$id/cancel');
    } on DioException catch (e) {
      throw ApiException.fromDioError(e);
    }
  }
}
