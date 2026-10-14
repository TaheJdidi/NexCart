import { apiClient } from '../../lib/apiClient'
import type { CreateStoreRequest, HandleAvailability, Store } from './types'

export const storesApi = {
  async create(request: CreateStoreRequest): Promise<Store> {
    const { data } = await apiClient.post<Store>('/stores', request)
    return data
  },

  async checkHandle(handle: string, signal?: AbortSignal): Promise<HandleAvailability> {
    const { data } = await apiClient.get<HandleAvailability>('/stores/handle-availability', {
      params: { handle },
      signal,
    })
    return data
  },
}
