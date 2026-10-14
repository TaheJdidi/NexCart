/** A Store as listed on the Merchant's account (returned by `me`). */
export interface OwnedStore {
  id: string
  name: string
  handle: string
}

export interface Store extends OwnedStore {
  currency: string
  status: 'Active'
  createdAtUtc: string
}

export interface CreateStoreRequest {
  name: string
  handle: string
  currency?: string
}

export type HandleUnavailableReason = 'taken' | 'reserved' | 'invalid'

export interface HandleAvailability {
  /** The handle as the server will store it (lowercased and trimmed). */
  handle: string
  available: boolean
  reason: HandleUnavailableReason | null
}
