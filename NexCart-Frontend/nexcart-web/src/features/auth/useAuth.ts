import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { authApi } from './authApi'
import type { User } from './types'

export const currentUserKey = ['auth', 'me'] as const

/** The signed-in user (null when signed out). The server is the source of truth, via the auth cookie. */
export function useCurrentUser() {
  return useQuery({
    queryKey: currentUserKey,
    queryFn: authApi.getCurrentUser,
    staleTime: Infinity,
  })
}

export function useLogin() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: authApi.login,
    onSuccess: (user) => queryClient.setQueryData<User | null>(currentUserKey, user),
  })
}

export function useRegister() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: authApi.register,
    onSuccess: (user) => queryClient.setQueryData<User | null>(currentUserKey, user),
  })
}

export function useLogout() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: authApi.logout,
    onSettled: () => {
      // Drop everything cached for the previous user, then mark the session as signed out
      queryClient.clear()
      queryClient.setQueryData<User | null>(currentUserKey, null)
    },
  })
}
