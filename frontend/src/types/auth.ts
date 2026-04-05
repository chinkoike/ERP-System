export interface LoginRequest {
  username: string
  password: string
}

export interface AuthResponse {
  token: string
  refreshToken: string
  expiresIn: number
  username: string
  email: string
  roles: string[]
}

export interface RefreshTokenRequest {
  refreshToken: string
}

export interface User {
  username: string
  email: string
  roles: string[]
}
