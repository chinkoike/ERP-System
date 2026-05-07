import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../data/models/purchasing_models.dart';
import '../../data/repositories/purchasing_repository.dart';

final suppliersListProvider = FutureProvider<List<SupplierModel>>((ref) {
  return ref.read(purchasingRepositoryProvider).getAllSuppliers();
});

class SupplierFormNotifier extends StateNotifier<AsyncValue<void>> {
  final PurchasingRepository _repo;
  SupplierFormNotifier(this._repo) : super(const AsyncValue.data(null));

  Future<bool> create({
    required String name,
    String? contactName,
    String? email,
    String? phone,
    String? address,
  }) async {
    state = const AsyncValue.loading();
    try {
      await _repo.createSupplier(
        name: name,
        contactName: contactName,
        email: email,
        phone: phone,
        address: address,
      );
      state = const AsyncValue.data(null);
      return true;
    } catch (e, st) {
      state = AsyncValue.error(e, st);
      return false;
    }
  }

  Future<bool> update(
    String id, {
    String? name,
    String? contactName,
    String? email,
    String? phone,
    String? address,
  }) async {
    state = const AsyncValue.loading();
    try {
      await _repo.updateSupplier(
        id,
        name: name,
        contactName: contactName,
        email: email,
        phone: phone,
        address: address,
      );
      state = const AsyncValue.data(null);
      return true;
    } catch (e, st) {
      state = AsyncValue.error(e, st);
      return false;
    }
  }

  Future<bool> delete(String id) async {
    state = const AsyncValue.loading();
    try {
      await _repo.deleteSupplier(id);
      state = const AsyncValue.data(null);
      return true;
    } catch (e, st) {
      state = AsyncValue.error(e, st);
      return false;
    }
  }
}

final supplierFormProvider =
    StateNotifierProvider<SupplierFormNotifier, AsyncValue<void>>((ref) {
  return SupplierFormNotifier(ref.read(purchasingRepositoryProvider));
});

class PurchaseOrderSearchState {
  final String? selectedSupplierId;
  final String? selectedStatus;
  final bool isLoading;
  final String? error;
  final List<PurchaseOrderModel> items;

  const PurchaseOrderSearchState({
    this.selectedSupplierId,
    this.selectedStatus,
    this.isLoading = false,
    this.error,
    this.items = const [],
  });

  PurchaseOrderSearchState copyWith({
    String? selectedSupplierId,
    String? selectedStatus,
    bool? isLoading,
    String? error,
    List<PurchaseOrderModel>? items,
  }) {
    return PurchaseOrderSearchState(
      selectedSupplierId: selectedSupplierId ?? this.selectedSupplierId,
      selectedStatus: selectedStatus ?? this.selectedStatus,
      isLoading: isLoading ?? this.isLoading,
      error: error,
      items: items ?? this.items,
    );
  }
}

class PurchaseOrderSearchNotifier extends StateNotifier<PurchaseOrderSearchState> {
  final PurchasingRepository _repo;
  PurchaseOrderSearchNotifier(this._repo) : super(const PurchaseOrderSearchState()) {
    search();
  }

  Future<void> search({
    String? supplierId,
    String? status,
    DateTime? startDate,
    DateTime? endDate,
  }) async {
    state = state.copyWith(
      selectedSupplierId: supplierId ?? state.selectedSupplierId,
      selectedStatus: status ?? state.selectedStatus,
      isLoading: true,
      error: null,
    );
    try {
      final result = await _repo.searchPurchaseOrders(
        supplierId: state.selectedSupplierId,
        status: state.selectedStatus,
        startDate: startDate,
        endDate: endDate,
      );
      state = state.copyWith(isLoading: false, items: result);
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e.toString());
    }
  }

  Future<bool> cancel(String purchaseOrderId) async {
    try {
      await _repo.cancelPurchaseOrder(purchaseOrderId);
      await search();
      return true;
    } catch (e) {
      state = state.copyWith(error: e.toString());
      return false;
    }
  }
}

final purchaseOrderSearchProvider =
    StateNotifierProvider<PurchaseOrderSearchNotifier, PurchaseOrderSearchState>(
        (ref) {
  return PurchaseOrderSearchNotifier(ref.read(purchasingRepositoryProvider));
});
