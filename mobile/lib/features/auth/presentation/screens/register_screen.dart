import 'package:flutter/material.dart';
import 'package:flutter_animate/flutter_animate.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../shared/widgets/app_text_field.dart';
import '../../../../shared/widgets/app_button.dart';
import '../../../../shared/widgets/app_snackbar.dart';
import '../providers/auth_provider.dart';

class RegisterScreen extends ConsumerStatefulWidget {
  const RegisterScreen({super.key});

  @override
  ConsumerState<RegisterScreen> createState() => _RegisterScreenState();
}

class _RegisterScreenState extends ConsumerState<RegisterScreen> {
  final _formKey = GlobalKey<FormState>();
  final _usernameCtrl = TextEditingController();
  final _emailCtrl = TextEditingController();
  final _passwordCtrl = TextEditingController();
  final _firstNameCtrl = TextEditingController();
  final _lastNameCtrl = TextEditingController();
  bool _obscure = true;

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
    final success = await ref.read(authProvider.notifier).register(
          username: _usernameCtrl.text.trim(),
          email: _emailCtrl.text.trim(),
          password: _passwordCtrl.text,
          firstName: _firstNameCtrl.text.trim().isEmpty
              ? null
              : _firstNameCtrl.text.trim(),
          lastName: _lastNameCtrl.text.trim().isEmpty
              ? null
              : _lastNameCtrl.text.trim(),
        );
    if (!mounted) return;
    if (success) {
      AppSnackbar.success(context, 'Account created! Please sign in.');
      context.pop();
    } else {
      final error = ref.read(authProvider).error;
      AppSnackbar.error(context, error ?? 'Registration failed');
    }
  }

  @override
  Widget build(BuildContext context) {
    final isLoading = ref.watch(authProvider).isLoading;

    return Scaffold(
      backgroundColor: AppColors.bg0,
      appBar: AppBar(
        title: const Text('Create Account'),
        leading: IconButton(
          icon: const Icon(Icons.arrow_back_ios_new_rounded, size: 18),
          onPressed: () => context.pop(),
        ),
      ),
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 16),
          child: Form(
            key: _formKey,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Fill in your details',
                  style: Theme.of(context).textTheme.bodyMedium,
                ).animate().fadeIn(),
                const SizedBox(height: 24),

                Row(
                  children: [
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
                  ],
                ).animate().fadeIn(delay: 50.ms),
                const SizedBox(height: 16),

                AppTextField(
                  controller: _usernameCtrl,
                  label: 'Username *',
                  hint: 'johndoe',
                  prefixIcon: Icons.person_outline_rounded,
                  validator: (v) =>
                      v == null || v.isEmpty ? 'Username is required' : null,
                ).animate().fadeIn(delay: 100.ms),
                const SizedBox(height: 16),

                AppTextField(
                  controller: _emailCtrl,
                  label: 'Email *',
                  hint: 'john@example.com',
                  prefixIcon: Icons.email_outlined,
                  keyboardType: TextInputType.emailAddress,
                  validator: (v) {
                    if (v == null || v.isEmpty) return 'Email is required';
                    if (!v.contains('@')) return 'Invalid email';
                    return null;
                  },
                ).animate().fadeIn(delay: 150.ms),
                const SizedBox(height: 16),

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
                    if (v == null || v.isEmpty) return 'Password is required';
                    if (v.length < 8) return 'Minimum 8 characters';
                    return null;
                  },
                ).animate().fadeIn(delay: 200.ms),
                const SizedBox(height: 32),

                AppButton(
                  label: 'Create Account',
                  isLoading: isLoading,
                  onPressed: isLoading ? null : _submit,
                ).animate().fadeIn(delay: 250.ms),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
