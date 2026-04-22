import 'package:flutter/material.dart';
import 'package:flutter_animate/flutter_animate.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../shared/widgets/app_text_field.dart';
import '../../../../shared/widgets/app_button.dart';
import '../../../../shared/widgets/app_snackbar.dart';
import '../providers/users_provider.dart';

class UserFormScreen extends ConsumerStatefulWidget {
  final String? userId; // null = create mode

  const UserFormScreen({super.key, this.userId});

  bool get isEdit => userId != null;

  @override
  ConsumerState<UserFormScreen> createState() => _UserFormScreenState();
}

class _UserFormScreenState extends ConsumerState<UserFormScreen> {
  final _formKey = GlobalKey<FormState>();
  final _usernameCtrl = TextEditingController();
  final _emailCtrl = TextEditingController();
  final _passwordCtrl = TextEditingController();
  final _firstNameCtrl = TextEditingController();
  final _lastNameCtrl = TextEditingController();
  bool _obscure = true;
  bool _isActive = true;

  @override
  void dispose() {
    _usernameCtrl.dispose();
    _emailCtrl.dispose();
    _passwordCtrl.dispose();
    _firstNameCtrl.dispose();
    _lastNameCtrl.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;
    final notifier = ref.read(userFormProvider.notifier);
    bool ok;

    if (widget.isEdit) {
      ok = await notifier.update(
        widget.userId!,
        firstName: _firstNameCtrl.text.trim().isEmpty ? null : _firstNameCtrl.text.trim(),
        lastName: _lastNameCtrl.text.trim().isEmpty ? null : _lastNameCtrl.text.trim(),
        email: _emailCtrl.text.trim(),
        isActive: _isActive,
      );
    } else {
      ok = await notifier.create(
        username: _usernameCtrl.text.trim(),
        email: _emailCtrl.text.trim(),
        password: _passwordCtrl.text,
        firstName: _firstNameCtrl.text.trim().isEmpty ? null : _firstNameCtrl.text.trim(),
        lastName: _lastNameCtrl.text.trim().isEmpty ? null : _lastNameCtrl.text.trim(),
      );
    }

    if (!mounted) return;
    if (ok) {
      ref.invalidate(userSearchProvider);
      AppSnackbar.success(
          context, widget.isEdit ? 'User updated' : 'User created');
      context.pop();
    } else {
      final state = ref.read(userFormProvider);
      state.whenOrNull(error: (e, _) => AppSnackbar.error(context, e.toString()));
    }
  }

  @override
  Widget build(BuildContext context) {
    final formState = ref.watch(userFormProvider);
    final isLoading = formState.isLoading;

    return Scaffold(
      backgroundColor: AppColors.bg0,
      appBar: AppBar(
        title: Text(widget.isEdit ? 'Edit User' : 'Create User'),
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
            children: [
              Row(children: [
                Expanded(
                  child: AppTextField(
                    controller: _firstNameCtrl,
                    label: 'First Name',
                    hint: 'John',
                    prefixIcon: Icons.badge_outlined,
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: AppTextField(
                    controller: _lastNameCtrl,
                    label: 'Last Name',
                    hint: 'Doe',
                    prefixIcon: Icons.badge_outlined,
                  ),
                ),
              ]).animate().fadeIn(delay: 50.ms),
              const SizedBox(height: 16),

              if (!widget.isEdit)
                AppTextField(
                  controller: _usernameCtrl,
                  label: 'Username *',
                  hint: 'johndoe',
                  prefixIcon: Icons.person_outline_rounded,
                  validator: (v) =>
                      v == null || v.isEmpty ? 'Required' : null,
                ).animate().fadeIn(delay: 100.ms),
              if (!widget.isEdit) const SizedBox(height: 16),

              AppTextField(
                controller: _emailCtrl,
                label: 'Email *',
                hint: 'john@example.com',
                prefixIcon: Icons.email_outlined,
                keyboardType: TextInputType.emailAddress,
                validator: (v) {
                  if (v == null || v.isEmpty) return 'Required';
                  if (!v.contains('@')) return 'Invalid email';
                  return null;
                },
              ).animate().fadeIn(delay: 150.ms),
              const SizedBox(height: 16),

              if (!widget.isEdit)
                AppTextField(
                  controller: _passwordCtrl,
                  label: 'Password *',
                  hint: 'Min 8 characters',
                  prefixIcon: Icons.lock_outline_rounded,
                  obscureText: _obscure,
                  suffixIcon: IconButton(
                    icon: Icon(
                      _obscure
                          ? Icons.visibility_outlined
                          : Icons.visibility_off_outlined,
                      color: AppColors.textSecondary,
                      size: 20,
                    ),
                    onPressed: () => setState(() => _obscure = !_obscure),
                  ),
                  validator: (v) {
                    if (v == null || v.isEmpty) return 'Required';
                    if (v.length < 8) return 'Minimum 8 characters';
                    return null;
                  },
                ).animate().fadeIn(delay: 200.ms),
              if (!widget.isEdit) const SizedBox(height: 16),

              if (widget.isEdit) ...[
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 4),
                  decoration: BoxDecoration(
                    color: AppColors.bg2,
                    borderRadius: BorderRadius.circular(10),
                    border: Border.all(color: AppColors.border),
                  ),
                  child: SwitchListTile(
                    contentPadding: EdgeInsets.zero,
                    title: const Text(
                      'Active',
                      style: TextStyle(fontFamily: 'Sora', color: AppColors.textPrimary),
                    ),
                    subtitle: const Text(
                      'Toggle user account status',
                      style: TextStyle(fontFamily: 'Sora', color: AppColors.textSecondary, fontSize: 12),
                    ),
                    value: _isActive,
                    activeColor: AppColors.accent,
                    onChanged: (v) => setState(() => _isActive = v),
                  ),
                ).animate().fadeIn(delay: 200.ms),
                const SizedBox(height: 16),
              ],

              AppButton(
                label: widget.isEdit ? 'Save Changes' : 'Create User',
                isLoading: isLoading,
                onPressed: isLoading ? null : _submit,
              ).animate().fadeIn(delay: 250.ms),
            ],
          ),
        ),
      ),
    );
  }
}
