import 'package:json_annotation/json_annotation.dart';

part 'purchasing_models.g.dart';

@JsonSerializable()
class SupplierModel {
  final String id;
  final String name;
  final String? contactName;
  final String? email;
  final String? phone;
  final String? address;
  final DateTime? createdAt;
  final DateTime? updatedAt;

  const SupplierModel({
    required this.id,
    required this.name,
    this.contactName,
    this.email,
    this.phone,
    this.address,
    this.createdAt,
    this.updatedAt,
  });

  factory SupplierModel.fromJson(Map<String, dynamic> json) =>
      _$SupplierModelFromJson(json);

  Map<String, dynamic> toJson() => _$SupplierModelToJson(this);
}

@JsonSerializable()
class PurchaseOrderItemModel {
  final String productId;
  final int quantityOrdered;
  final int quantityReceived;
  final double unitPrice;
  final double totalPrice;

  const PurchaseOrderItemModel({
    required this.productId,
    required this.quantityOrdered,
    required this.quantityReceived,
    required this.unitPrice,
    required this.totalPrice,
  });

  factory PurchaseOrderItemModel.fromJson(Map<String, dynamic> json) =>
      _$PurchaseOrderItemModelFromJson(json);

  Map<String, dynamic> toJson() => _$PurchaseOrderItemModelToJson(this);
}

@JsonSerializable()
class PurchaseOrderModel {
  final String id;
  final String purchaseOrderNumber;
  final String supplierId;
  final String supplierName;
  final DateTime? orderDate;
  final String status;
  final double totalAmount;
  final List<PurchaseOrderItemModel> items;

  const PurchaseOrderModel({
    required this.id,
    required this.purchaseOrderNumber,
    required this.supplierId,
    required this.supplierName,
    required this.orderDate,
    required this.status,
    required this.totalAmount,
    required this.items,
  });

  factory PurchaseOrderModel.fromJson(Map<String, dynamic> json) =>
      _$PurchaseOrderModelFromJson(json);

  Map<String, dynamic> toJson() => _$PurchaseOrderModelToJson(this);
}

@JsonSerializable()
class PurchaseOrderSearchResponseModel {
  final List<PurchaseOrderModel> items;
  final int totalCount;
  @JsonKey(name: 'pageNumber')
  final int page;
  final int pageSize;
  final int totalPages;

  const PurchaseOrderSearchResponseModel({
    required this.items,
    required this.totalCount,
    required this.page,
    required this.pageSize,
    required this.totalPages,
  });

  factory PurchaseOrderSearchResponseModel.fromJson(Map<String, dynamic> json) =>
      _$PurchaseOrderSearchResponseModelFromJson(json);

  Map<String, dynamic> toJson() => _$PurchaseOrderSearchResponseModelToJson(this);
}
