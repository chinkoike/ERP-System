import 'package:flutter/material.dart';
import 'package:flutter_animate/flutter_animate.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../shared/widgets/app_text_field.dart';
import '../../../../shared/widgets/app_button.dart';
import '../../../../shared/widgets/app_snackbar.dart';
import '../providers/roles_provider.dart';
import '../../../auth/data/models/auth_models.dart';
import '../../data/repositories/roles_repository.dart';

final _roleDetailProvider =
    FutureProvider.family<RoleModel, String>((ref, id) {
  return ref.read(rolesRepositoryProvider).getRoleById(id);
});

class RoleFormScreen extends ConsumerStatefulWidget {
  final String? roleId;
  const RoleFormScreen({super.key, this.roleId});

  bool get isEdit => roleId != null;

  @override
  ConsumerState<RoleFormScreen> createState() => _RoleFormScreenState();
}

class _RoleFormScreenState extends ConsumerState<RoleFormScreen> {
  final _formKey = GlobalKey<FormState>();
  final _nameCtrl = TextEditingController();
  final _descCtrl = TextEditingController();
  bool _loaded = false;

  @override
  void dispose() {
    _nameCtrl.dispose();
    _descCtrl.dispose();
    super.dispose();
  }

  void _prefill(RoleModel role) {
    if (!_loaded) {
      _nameCtrl.text = role.name;
      _descCtrl.text = role.description ?? '';
      _loaded = true;
    }
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;
    final notifier = ref.read(roleFormProvider.notifier);
    final name = _nameCtrl.text.trim();
    final desc = _descCtrl.text.trim().isEmpty ? null : _descCtrl.text.trim();

    final ok = widget.isEdit
        ? await notifier.update(widget.roleId!, name, desc)
        : await notifier.create(name, desc);

    if (!mounted) return;
    if (ok) {
      AppSnackbar.success(context, widget.isEdit ? 'Role updated' : 'Role created');
      context.pop();
    } else {
      final state = ref.read(roleFormProvider);
      state.whenOrNull(error: (e, _) => AppSnackbar.error(context, e.toString()));
    }
  }

  @override
  Widget build(BuildContext context) {
    final formState = ref.watch(roleFormProvider);
    final isLoading = formState.isLoading;

    Widget body = SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Form(
        key: _formKey,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            AppTextField(
              controller: _nameCtrl,
              label: 'Role Name *',
              hint: 'e.g. Admin, Manager',
              prefixIcon: Icons.shield_outlined,
              validator: (v) =>
                  v == null || v.trim().isEmpty ? 'Role name is required' : null,
            ).animate().fadeIn(delay: 50.ms),
            const SizedBox(height: 16),
            AppTextField(
              controller: _descCtrl,
              label: 'Description',
              hint: 'Optional description of this role',
              prefixIcon: Icons.notes_rounded,
              maxLines: 3,
            ).animate().fadeIn(delay: 100.ms),
            const SizedBox(height: 32),
            AppButton(
              label: widget.isEdit ? 'Save Changes' : 'Create Role',
              isLoading: isLoading,
              onPressed: isLoading ? null : _submit,
            ).animate().fadeIn(delay: 150.ms),
          ],
        ),
      ),
    );

    // If edit mode, prefill from API
    if (widget.isEdit) {
      final roleAsync = ref.watch(_roleDetailProvider(widget.roleId!));
      roleAsync.whenData(_prefill);
    }

    return Scaffold(
      backgroundColor: AppColors.bg0,
      appBar: AppBar(
        title: Text(widget.isEdit ? 'Edit Role' : 'Create Role'),
        leading: IconButton(
          icon: const Icon(Icons.arrow_back_ios_new_rounded, size: 18),
          onPressed: () => context.pop(),
        ),
      ),
      body: body,
    );
  }
}
