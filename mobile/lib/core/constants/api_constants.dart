class ApiConstants {
  static const String baseUrl = 'http://localhost:5049/api';

  // Auth endpoints
  static const String login = '/users/login';
  static const String register = '/users/register';
  static const String refreshToken = '/users/refresh-token';
  static const String logout = '/users/logout';

  // Users endpoints
  static const String users = '/users';
  static const String usersActive = '/users/active';
  static const String usersSearch = '/users/search';

  // Roles endpoints
  static const String roles = '/roles';

  // Purchasing endpoints
  static const String purchasing = '/purchasing';
  static const String purchasingSuppliers = '/purchasing/suppliers';
  static const String purchaseOrders = '/purchasing/purchase-orders';
  static const String purchaseOrdersSearch = '/purchasing/purchase-orders/search';

  // Timeouts
  static const Duration connectTimeout = Duration(seconds: 30);
  static const Duration receiveTimeout = Duration(seconds: 30);
}

class StorageKeys {
  static const String accessToken = 'access_token';
  static const String refreshToken = 'refresh_token';
  static const String userId = 'user_id';
  static const String username = 'username';
  static const String userRole = 'user_role';
}
