import { useEffect, useState } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { currentUserKey } from '../auth/useAuth'
import { isWellFormedHandle } from './schemas'
import { storesApi } from './storesApi'

export function useCreateStore() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: storesApi.create,
    // `me` lists the account's Stores, so it must pick up the new one
    onSuccess: () => queryClient.invalidateQueries({ queryKey: currentUserKey }),
  })
}

const checkDelayMs = 400

/**
 * Asks the server whether a handle is free, once the Merchant stops typing.
 * Handles that cannot be valid are not sent; the form already explains those.
 */
export function useHandleAvailability(input: string) {
  const handle = input.trim().toLowerCase()
  const [debounced, setDebounced] = useState(handle)

  useEffect(() => {
    const timer = setTimeout(() => setDebounced(handle), checkDelayMs)
    return () => clearTimeout(timer)
  }, [handle])

  const checkable = isWellFormedHandle(debounced)

  const query = useQuery({
    queryKey: ['stores', 'handle-availability', debounced],
    queryFn: ({ signal }) => storesApi.checkHandle(debounced, signal),
    enabled: checkable,
    staleTime: 10_000,
  })

  return {
    // Only report on the handle currently typed, not on one from before the last keystroke
    availability: checkable && debounced === handle ? query.data : undefined,
    isChecking: handle !== debounced || (checkable && query.isFetching),
  }
}
