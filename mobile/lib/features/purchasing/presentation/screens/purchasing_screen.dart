import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../shared/widgets/app_snackbar.dart';
import '../../../../shared/widgets/common_widgets.dart';
import '../../data/models/purchasing_models.dart';
import '../providers/purchasing_provider.dart';

class PurchasingScreen extends ConsumerStatefulWidget {
  const PurchasingScreen({super.key});

  @override
  ConsumerState<PurchasingScreen> createState() => _PurchasingScreenState();
}

class _PurchasingScreenState extends ConsumerState<PurchasingScreen>
    with SingleTickerProviderStateMixin {
  late final TabController _tabController;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.bg0,
      appBar: AppBar(
        title: const Text('Purchasing'),
        actions: [
          IconButton(
            icon: const Icon(Icons.add_business_outlined, size: 22),
            tooltip: 'Create Supplier',
            onPressed: () async {
              await context.push('/purchasing/suppliers/create');
              ref.invalidate(suppliersListProvider);
            },
          ),
        ],
        bottom: TabBar(
          controller: _tabController,
          tabs: const [
            Tab(text: 'Purchase Orders'),
            Tab(text: 'Suppliers'),
          ],
        ),
      ),
      body: TabBarView(
        controller: _tabController,
        children: const [
          _PurchaseOrdersTab(),
          _SuppliersTab(),
        ],
      ),
    );
  }
}

class _SuppliersTab extends ConsumerWidget {
  const _SuppliersTab();

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final suppliersAsync = ref.watch(suppliersListProvider);
    return suppliersAsync.when(
      loading: () => const Center(
          child: CircularProgressIndicator(color: AppColors.accent)),
      error: (e, _) => ErrorState(
        message: e.toString(),
        onRetry: () => ref.invalidate(suppliersListProvider),
      ),
      data: (suppliers) {
        if (suppliers.isEmpty) {
          return EmptyState(
            icon: Icons.store_outlined,
            title: 'No suppliers yet',
            subtitle: 'Create your first supplier to start purchasing',
            action: ElevatedButton.icon(
              onPressed: () => context.push('/purchasing/suppliers/create'),
              icon: const Icon(Icons.add_rounded, size: 16),
              label: const Text('Create Supplier'),
            ),
          );
        }
        return ListView.separated(
          padding: const EdgeInsets.fromLTRB(16, 12, 16, 80),
          itemCount: suppliers.length,
          separatorBuilder: (_, __) => const SizedBox(height: 8),
          itemBuilder: (_, i) => _SupplierCard(supplier: suppliers[i]),
        );
      },
    );
  }
}

class _SupplierCard extends ConsumerWidget {
  final SupplierModel supplier;
  const _SupplierCard({required this.supplier});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: AppColors.bg2,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: AppColors.border),
      ),
      child: Row(
        children: [
          const Icon(Icons.store_outlined, color: AppColors.accent),
          const SizedBox(width: 10),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  supplier.name,
                  style: const TextStyle(
                    fontFamily: 'Sora',
                    fontWeight: FontWeight.w600,
                    color: AppColors.textPrimary,
                  ),
                ),
                const SizedBox(height: 2),
                Text(
                  supplier.contactName ??
                      supplier.email ??
                      supplier.phone ??
                      '-',
                  style: const TextStyle(
                    fontFamily: 'Sora',
                    fontSize: 12,
                    color: AppColors.textSecondary,
                  ),
                ),
              ],
            ),
          ),
          PopupMenuButton<String>(
            color: AppColors.bg3,
            onSelected: (value) async {
              if (value == 'edit') {
                await context.push('/purchasing/suppliers/${supplier.id}/edit');
                ref.invalidate(suppliersListProvider);
              } else if (value == 'delete') {
                final ok = await ref
                    .read(supplierFormProvider.notifier)
                    .delete(supplier.id);
                if (context.mounted) {
                  if (ok) {
                    ref.invalidate(suppliersListProvider);
                    AppSnackbar.success(context, 'Supplier deleted');
                  } else {
                    AppSnackbar.error(context, 'Delete supplier failed');
                  }
                }
              }
            },
            itemBuilder: (_) => const [
              PopupMenuItem(value: 'edit', child: Text('Edit')),
              PopupMenuItem(value: 'delete', child: Text('Delete')),
            ],
          ),
        ],
      ),
    );
  }
}

class _PurchaseOrdersTab extends ConsumerWidget {
  const _PurchaseOrdersTab();

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final poState = ref.watch(purchaseOrderSearchProvider);
    if (poState.isLoading && poState.items.isEmpty) {
      return const Center(
          child: CircularProgressIndicator(color: AppColors.accent));
    }
    if (poState.error != null && poState.items.isEmpty) {
      return ErrorState(
        message: poState.error!,
        onRetry: () => ref.read(purchaseOrderSearchProvider.notifier).search(),
      );
    }
    if (poState.items.isEmpty) {
      return const EmptyState(
        icon: Icons.receipt_long_outlined,
        title: 'No purchase orders',
        subtitle: 'Purchase orders will appear here',
      );
    }

    return ListView.separated(
      padding: const EdgeInsets.fromLTRB(16, 12, 16, 80),
      itemCount: poState.items.length,
      separatorBuilder: (_, __) => const SizedBox(height: 8),
      itemBuilder: (_, i) => _PurchaseOrderCard(po: poState.items[i]),
    );
  }
}

class _PurchaseOrderCard extends ConsumerWidget {
  final PurchaseOrderModel po;
  const _PurchaseOrderCard({required this.po});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: AppColors.bg2,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: AppColors.border),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Expanded(
                child: Text(
                  po.purchaseOrderNumber,
                  style: const TextStyle(
                    fontFamily: 'Sora',
                    fontWeight: FontWeight.w600,
                    color: AppColors.textPrimary,
                  ),
                ),
              ),
              Text(
                po.status,
                style: const TextStyle(
                  fontFamily: 'Sora',
                  fontSize: 12,
                  color: AppColors.textSecondary,
                ),
              ),
            ],
          ),
          const SizedBox(height: 4),
          Text(
            '${po.supplierName} - ${po.totalAmount.toStringAsFixed(2)}',
            style: const TextStyle(
              fontFamily: 'Sora',
              fontSize: 12,
              color: AppColors.textSecondary,
            ),
          ),
        ],
      ),
    );
  }
}
