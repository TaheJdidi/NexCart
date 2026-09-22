import axios from 'axios'
import { apiClient } from '../../lib/apiClient'
import type { LoginRequest, RegisterRequest, User } from './types'

export const authApi = {
  async login(request: LoginRequest): Promise<User> {
    const { data } = await apiClient.post<User>('/auth/login', request)
    return data
  },

  async register(request: RegisterRequest): Promise<User> {
    const { data } = await apiClient.post<User>('/auth/register', request)
    return data
  },

  async logout(): Promise<void> {
    await apiClient.post('/auth/logout')
  },

  /** Returns the signed-in user, or null when there is no valid session. */
  async getCurrentUser(): Promise<User | null> {
    try {
      const { data } = await apiClient.get<User>('/auth/me')
      return data
    } catch (error) {
      if (axios.isAxiosError(error) && error.response?.status === 401) {
        return null
      }
      throw error
    }
  },
}
