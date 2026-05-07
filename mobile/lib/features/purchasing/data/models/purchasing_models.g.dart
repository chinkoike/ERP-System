// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'purchasing_models.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SupplierModel _$SupplierModelFromJson(Map<String, dynamic> json) =>
    SupplierModel(
      id: json['id'] as String,
      name: json['name'] as String,
      contactName: json['contactName'] as String?,
      email: json['email'] as String?,
      phone: json['phone'] as String?,
      address: json['address'] as String?,
      createdAt: json['createdAt'] == null
          ? null
          : DateTime.parse(json['createdAt'] as String),
      updatedAt: json['updatedAt'] == null
          ? null
          : DateTime.parse(json['updatedAt'] as String),
    );

Map<String, dynamic> _$SupplierModelToJson(SupplierModel instance) =>
    <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'contactName': instance.contactName,
      'email': instance.email,
      'phone': instance.phone,
      'address': instance.address,
      'createdAt': instance.createdAt?.toIso8601String(),
      'updatedAt': instance.updatedAt?.toIso8601String(),
    };

PurchaseOrderItemModel _$PurchaseOrderItemModelFromJson(
        Map<String, dynamic> json) =>
    PurchaseOrderItemModel(
      productId: json['productId'] as String,
      quantityOrdered: (json['quantityOrdered'] as num).toInt(),
      quantityReceived: (json['quantityReceived'] as num).toInt(),
      unitPrice: (json['unitPrice'] as num).toDouble(),
      totalPrice: (json['totalPrice'] as num).toDouble(),
    );

Map<String, dynamic> _$PurchaseOrderItemModelToJson(
        PurchaseOrderItemModel instance) =>
    <String, dynamic>{
      'productId': instance.productId,
      'quantityOrdered': instance.quantityOrdered,
      'quantityReceived': instance.quantityReceived,
      'unitPrice': instance.unitPrice,
      'totalPrice': instance.totalPrice,
    };

PurchaseOrderModel _$PurchaseOrderModelFromJson(Map<String, dynamic> json) =>
    PurchaseOrderModel(
      id: json['id'] as String,
      purchaseOrderNumber: json['purchaseOrderNumber'] as String,
      supplierId: json['supplierId'] as String,
      supplierName: json['supplierName'] as String,
      orderDate: json['orderDate'] == null
          ? null
          : DateTime.parse(json['orderDate'] as String),
      status: json['status'] as String,
      totalAmount: (json['totalAmount'] as num).toDouble(),
      items: (json['items'] as List<dynamic>)
          .map(
              (e) => PurchaseOrderItemModel.fromJson(e as Map<String, dynamic>))
          .toList(),
    );

Map<String, dynamic> _$PurchaseOrderModelToJson(PurchaseOrderModel instance) =>
    <String, dynamic>{
      'id': instance.id,
      'purchaseOrderNumber': instance.purchaseOrderNumber,
      'supplierId': instance.supplierId,
      'supplierName': instance.supplierName,
      'orderDate': instance.orderDate?.toIso8601String(),
      'status': instance.status,
      'totalAmount': instance.totalAmount,
      'items': instance.items,
    };

PurchaseOrderSearchResponseModel _$PurchaseOrderSearchResponseModelFromJson(
        Map<String, dynamic> json) =>
    PurchaseOrderSearchResponseModel(
      items: (json['items'] as List<dynamic>)
          .map((e) => PurchaseOrderModel.fromJson(e as Map<String, dynamic>))
          .toList(),
      totalCount: (json['totalCount'] as num).toInt(),
      page: (json['pageNumber'] as num).toInt(),
      pageSize: (json['pageSize'] as num).toInt(),
      totalPages: (json['totalPages'] as num).toInt(),
    );

Map<String, dynamic> _$PurchaseOrderSearchResponseModelToJson(
        PurchaseOrderSearchResponseModel instance) =>
    <String, dynamic>{
      'items': instance.items,
      'totalCount': instance.totalCount,
      'pageNumber': instance.page,
      'pageSize': instance.pageSize,
      'totalPages': instance.totalPages,
    };
