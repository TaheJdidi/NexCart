import type { OwnedStore } from '../stores/types'

export interface User {
  id: string
  email: string
  firstName: string
  lastName: string
  /** The Stores this account owns as a Merchant. */
  stores: OwnedStore[]
}

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  firstName: string
  lastName: string
  email: string
  password: string
}
