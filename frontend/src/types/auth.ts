export interface LoginRequest {
  username: string
  password: string
}

export interface AuthUser {
  id?: string
  username: string
  email: string
  firstName?: string
  lastName?: string
  fullName?: string
  jobTitle?: string
  department?: string
  isActive?: boolean
  lastLoginAt?: string
  roles: string[]
  createdAt?: string
  createdBy?: string
  updatedAt?: string
  updatedBy?: string
}

export interface AuthResponse {
  token?: string
  refreshToken?: string
  expiresIn?: number
  isSuccess?: boolean
  message?: string
  user?: AuthUser
  roles?: string[]
}

export interface RefreshTokenRequest {
  accessToken: string
  refreshToken: string
}

export interface User {
  username: string
  email: string
  roles: string[]
}
