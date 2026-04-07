export interface User {
  id: string
  username: string
  email: string
  firstName?: string
  lastName?: string
  fullName: string
  jobTitle?: string
  department?: string
  isActive: boolean
  lastLoginAt?: string
  roles: string[]
  createdAt: string
  createdBy: string
  updatedAt?: string
  updatedBy?: string
}

export interface CreateUserPayload {
  username: string
  email: string
  password: string
  firstName: string
  lastName: string
  roleId?: string
  jobTitle?: string
  department?: string
}

export interface UpdateUserPayload {
  email: string
  firstName?: string
  lastName?: string
  isActive: boolean
  roleId?: string
  jobTitle?: string
  department?: string
}

export interface Role {
  id: string
  name: string
  description?: string
  createdAt: string
  createdBy: string
  updatedAt?: string
  updatedBy?: string
}

export interface CreateRolePayload {
  name: string
  description?: string
}

export interface UpdateRolePayload {
  name: string
  description?: string
}
