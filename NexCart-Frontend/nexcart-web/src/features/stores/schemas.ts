import { z } from 'zod'

// Mirrors the backend rules (StoreHandle / CreateStoreCommandValidator); the server has the final say
export const handlePattern = /^[a-z0-9]+(-[a-z0-9]+)*$/

/** Whether a (lowercased, trimmed) handle meets the format rules; reserved and taken handles still need the server. */
export function isWellFormedHandle(handle: string) {
  return handle.length >= 3 && handle.length <= 40 && handlePattern.test(handle)
}

export const createStoreSchema = z.object({
  name: z.string().trim().min(1, 'Store name is required').max(100),
  handle: z
    .string()
    .trim()
    .toLowerCase()
    .min(3, 'Use at least 3 characters')
    .max(40, 'Use at most 40 characters')
    .regex(handlePattern, 'Use lowercase letters, digits and single hyphens, not starting or ending with a hyphen'),
  currency: z.string().length(3),
})

export type CreateStoreFormValues = z.infer<typeof createStoreSchema>

/** Currencies offered in the form; the server accepts any ISO 4217 code. */
export const currencyOptions = [
  { code: 'TND', label: 'Tunisian dinar (TND)' },
  { code: 'EUR', label: 'Euro (EUR)' },
  { code: 'USD', label: 'US dollar (USD)' },
  { code: 'DZD', label: 'Algerian dinar (DZD)' },
  { code: 'MAD', label: 'Moroccan dirham (MAD)' },
  { code: 'LYD', label: 'Libyan dinar (LYD)' },
] as const
