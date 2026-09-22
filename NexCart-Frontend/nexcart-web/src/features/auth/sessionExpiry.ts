import axios from 'axios'
import { apiClient } from '../../lib/apiClient'
import { queryClient } from '../../lib/queryClient'
import { currentUserKey } from './useAuth'

/**
 * When any API call returns 401 (e.g. the token expired), mark the user as signed out
 * so protected routes redirect to the login page.
 */
export function installSessionExpiryHandler() {
  apiClient.interceptors.response.use(undefined, (error: unknown) => {
    const isAuthCall = axios.isAxiosError(error) && error.config?.url?.startsWith('/auth/')

    if (axios.isAxiosError(error) && error.response?.status === 401 && !isAuthCall) {
      queryClient.setQueryData(currentUserKey, null)
    }

    return Promise.reject(error)
  })
}
