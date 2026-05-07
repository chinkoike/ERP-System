import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../datasources/purchasing_remote_datasource.dart';
import '../models/purchasing_models.dart';

final purchasingRepositoryProvider = Provider<PurchasingRepository>((ref) {
  return PurchasingRepository(ref.read(purchasingRemoteDataSourceProvider));
});

class PurchasingRepository {
  final PurchasingRemoteDataSource _ds;
  PurchasingRepository(this._ds);

  Future<List<SupplierModel>> getAllSuppliers() => _ds.getAllSuppliers();
  Future<SupplierModel> getSupplierById(String id) => _ds.getSupplierById(id);

  Future<String> createSupplier({
    required String name,
    String? contactName,
    String? email,
    String? phone,
    String? address,
  }) =>
      _ds.createSupplier(
        name: name,
        contactName: contactName,
        email: email,
        phone: phone,
        address: address,
      );

  Future<void> updateSupplier(
    String id, {
    String? name,
    String? contactName,
    String? email,
    String? phone,
    String? address,
  }) =>
      _ds.updateSupplier(
        id,
        name: name,
        contactName: contactName,
        email: email,
        phone: phone,
        address: address,
      );

  Future<void> deleteSupplier(String id) => _ds.deleteSupplier(id);

  Future<List<PurchaseOrderModel>> getAllPurchaseOrders() =>
      _ds.getAllPurchaseOrders();

  Future<List<PurchaseOrderModel>> searchPurchaseOrders({
    String? supplierId,
    String? status,
    DateTime? startDate,
    DateTime? endDate,
  }) =>
      _ds.searchPurchaseOrders(
        supplierId: supplierId,
        status: status,
        startDate: startDate,
        endDate: endDate,
      );

  Future<PurchaseOrderModel> getPurchaseOrderById(String id) =>
      _ds.getPurchaseOrderById(id);

  Future<String> createPurchaseOrder({
    required String supplierId,
    required List<PurchaseOrderItemModel> items,
  }) =>
      _ds.createPurchaseOrder(supplierId: supplierId, items: items);

  Future<void> receivePurchaseOrder(
    String id,
    List<PurchaseOrderItemModel> items,
  ) =>
      _ds.receivePurchaseOrder(id, items);

  Future<void> cancelPurchaseOrder(String id) => _ds.cancelPurchaseOrder(id);
}
