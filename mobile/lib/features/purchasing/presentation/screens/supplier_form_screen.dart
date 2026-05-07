import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../shared/widgets/app_button.dart';
import '../../../../shared/widgets/app_snackbar.dart';
import '../../../../shared/widgets/app_text_field.dart';
import '../../data/models/purchasing_models.dart';
import '../../data/repositories/purchasing_repository.dart';
import '../providers/purchasing_provider.dart';

final _supplierDetailProvider =
    FutureProvider.family<SupplierModel, String>((ref, id) {
  return ref.read(purchasingRepositoryProvider).getSupplierById(id);
});

class SupplierFormScreen extends ConsumerStatefulWidget {
  final String? supplierId;
  const SupplierFormScreen({super.key, this.supplierId});

  bool get isEdit => supplierId != null;

  @override
  ConsumerState<SupplierFormScreen> createState() => _SupplierFormScreenState();
}

class _SupplierFormScreenState extends ConsumerState<SupplierFormScreen> {
  final _formKey = GlobalKey<FormState>();
  final _nameCtrl = TextEditingController();
  final _contactCtrl = TextEditingController();
  final _emailCtrl = TextEditingController();
  final _phoneCtrl = TextEditingController();
  final _addressCtrl = TextEditingController();
  bool _loaded = false;

  @override
  void dispose() {
    _nameCtrl.dispose();
    _contactCtrl.dispose();
    _emailCtrl.dispose();
    _phoneCtrl.dispose();
    _addressCtrl.dispose();
    super.dispose();
  }

  void _prefill(SupplierModel supplier) {
    if (_loaded) return;
    _nameCtrl.text = supplier.name;
    _contactCtrl.text = supplier.contactName ?? '';
    _emailCtrl.text = supplier.email ?? '';
    _phoneCtrl.text = supplier.phone ?? '';
    _addressCtrl.text = supplier.address ?? '';
    _loaded = true;
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;

    final notifier = ref.read(supplierFormProvider.notifier);
    final ok = widget.isEdit
        ? await notifier.update(
            widget.supplierId!,
            name: _nameCtrl.text.trim(),
            contactName: _contactCtrl.text.trim().isEmpty
                ? null
                : _contactCtrl.text.trim(),
            email: _emailCtrl.text.trim().isEmpty ? null : _emailCtrl.text.trim(),
            phone: _phoneCtrl.text.trim().isEmpty ? null : _phoneCtrl.text.trim(),
            address: _addressCtrl.text.trim().isEmpty
                ? null
                : _addressCtrl.text.trim(),
          )
        : await notifier.create(
            name: _nameCtrl.text.trim(),
            contactName: _contactCtrl.text.trim().isEmpty
                ? null
                : _contactCtrl.text.trim(),
            email: _emailCtrl.text.trim().isEmpty ? null : _emailCtrl.text.trim(),
            phone: _phoneCtrl.text.trim().isEmpty ? null : _phoneCtrl.text.trim(),
            address: _addressCtrl.text.trim().isEmpty
                ? null
                : _addressCtrl.text.trim(),
          );

    if (!mounted) return;
    if (ok) {
      AppSnackbar.success(
          context, widget.isEdit ? 'Supplier updated' : 'Supplier created');
      context.pop();
    } else {
      final state = ref.read(supplierFormProvider);
      state.whenOrNull(error: (e, _) => AppSnackbar.error(context, e.toString()));
    }
  }

  @override
  Widget build(BuildContext context) {
    final formState = ref.watch(supplierFormProvider);
    final isLoading = formState.isLoading;

    if (widget.isEdit) {
      ref.watch(_supplierDetailProvider(widget.supplierId!)).whenData(_prefill);
    }

    return Scaffold(
      backgroundColor: AppColors.bg0,
      appBar: AppBar(
        title: Text(widget.isEdit ? 'Edit Supplier' : 'Create Supplier'),
        leading: IconButton(
          icon: const Icon(Icons.arrow_back_ios_new_rounded, size: 18),
          onPressed: () => context.pop(),
        ),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              AppTextField(
                controller: _nameCtrl,
                label: 'Supplier Name *',
                hint: 'Supplier company name',
                prefixIcon: Icons.store_outlined,
                validator: (v) => v == null || v.trim().isEmpty
                    ? 'Supplier name is required'
                    : null,
              ),
              const SizedBox(height: 12),
              AppTextField(
                controller: _contactCtrl,
                label: 'Contact Name',
                hint: 'Contact person',
                prefixIcon: Icons.person_outline_rounded,
              ),
              const SizedBox(height: 12),
              AppTextField(
                controller: _emailCtrl,
                label: 'Email',
                hint: 'email@example.com',
                prefixIcon: Icons.email_outlined,
              ),
              const SizedBox(height: 12),
              AppTextField(
                controller: _phoneCtrl,
                label: 'Phone',
                hint: '+66...',
                prefixIcon: Icons.phone_outlined,
              ),
              const SizedBox(height: 12),
              AppTextField(
                controller: _addressCtrl,
                label: 'Address',
                hint: 'Supplier address',
                prefixIcon: Icons.location_on_outlined,
                maxLines: 3,
              ),
              const SizedBox(height: 28),
              AppButton(
                label: widget.isEdit ? 'Save Changes' : 'Create Supplier',
                isLoading: isLoading,
                onPressed: isLoading ? null : _submit,
              ),
            ],
          ),
        ),
      ),
    );
  }
}
