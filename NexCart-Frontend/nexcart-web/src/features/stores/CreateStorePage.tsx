import { useState } from 'react'
import { Controller, useForm, useWatch } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { useNavigate } from 'react-router-dom'
import { Alert, Box, Button, InputAdornment, MenuItem, Paper, Stack, TextField, Typography } from '@mui/material'
import toast from 'react-hot-toast'
import { toApiError } from '../../lib/apiClient'
import { createStoreSchema, currencyOptions, isWellFormedHandle, type CreateStoreFormValues } from './schemas'
import { platformDomain, storeAddress } from './storeAddress'
import type { HandleUnavailableReason } from './types'
import { useCreateStore, useHandleAvailability } from './useStores'

const serverFields = ['name', 'handle', 'currency'] as const

const handleHint = 'Your Store’s address. It cannot be changed later.'

const unavailableMessages: Record<HandleUnavailableReason, string> = {
  taken: 'This handle is already taken.',
  reserved: 'This handle is reserved by NexCart.',
  invalid: 'Use lowercase letters, digits and single hyphens, not starting or ending with a hyphen',
}

export function CreateStorePage() {
  const navigate = useNavigate()
  const createStore = useCreateStore()
  const [formError, setFormError] = useState<string | null>(null)

  const {
    register,
    control,
    handleSubmit,
    setError,
    formState: { errors },
  } = useForm<CreateStoreFormValues>({
    resolver: zodResolver(createStoreSchema),
    defaultValues: { name: '', handle: '', currency: 'TND' },
    mode: 'onTouched',
  })

  const handleInput = useWatch({ control, name: 'handle' })
  const { availability, isChecking } = useHandleAvailability(handleInput)

  const onSubmit = handleSubmit(async (values) => {
    setFormError(null)
    try {
      const store = await createStore.mutateAsync(values)
      toast.success(`${store.name} is ready!`)
      navigate('/stores', { replace: true })
    } catch (error) {
      const apiError = toApiError(error)
      let shownOnField = false

      // Show server-side errors (e.g. a handle taken a moment ago) next to the matching field
      for (const field of serverFields) {
        const message = apiError.fieldErrors[field]
        if (message) {
          setError(field, { message })
          shownOnField = true
        }
      }

      if (!shownOnField) {
        setFormError(apiError.message)
      }
    }
  })

  // Live feedback under the handle field: form rule errors first, then what the server says
  const handleFeedback = (): { error: boolean; text: string } => {
    if (errors.handle?.message) return { error: true, text: errors.handle.message }
    const handle = handleInput.trim().toLowerCase()
    if (!handle) return { error: false, text: handleHint }
    // Long enough to judge, and already breaking the format: say so before the field loses focus
    if (handle.length >= 3 && !isWellFormedHandle(handle)) return { error: true, text: unavailableMessages.invalid }
    if (isChecking) return { error: false, text: 'Checking…' }
    if (availability?.available) return { error: false, text: `${storeAddress(availability.handle)} is available` }
    if (availability?.reason) return { error: true, text: unavailableMessages[availability.reason] }
    return { error: false, text: handleHint }
  }

  const feedback = handleFeedback()
  const { ref: nameRef, ...nameField } = register('name')
  const { ref: handleRef, ...handleField } = register('handle')

  return (
    <Box sx={{ display: 'flex', justifyContent: 'center', py: { xs: 2, sm: 6 } }}>
      <Paper variant="outlined" sx={{ p: { xs: 3, sm: 4 }, width: '100%', maxWidth: 520 }}>
        <Typography component="h1" variant="h5" sx={{ fontWeight: 600 }}>
          Create a Store
        </Typography>
        <Typography color="text.secondary" sx={{ mt: 0.5, mb: 3 }}>
          Your Store gets its own site where Customers can browse and order.
        </Typography>

        <Box component="form" onSubmit={onSubmit} noValidate>
          <Stack spacing={2.5}>
            {formError && <Alert severity="error">{formError}</Alert>}

            <TextField
              {...nameField}
              inputRef={nameRef}
              label="Store name"
              autoFocus
              fullWidth
              error={!!errors.name}
              helperText={errors.name?.message ?? 'Shown to Customers. You can change it later.'}
            />

            <TextField
              {...handleField}
              inputRef={handleRef}
              label="Store handle"
              placeholder="ahmed-shoes"
              fullWidth
              autoComplete="off"
              slotProps={{
                htmlInput: { autoCapitalize: 'none', spellCheck: false },
                input: {
                  endAdornment: <InputAdornment position="end">.{platformDomain}</InputAdornment>,
                },
              }}
              error={feedback.error}
              helperText={feedback.text}
            />

            <Controller
              name="currency"
              control={control}
              render={({ field }) => (
                <TextField
                  {...field}
                  select
                  label="Currency"
                  fullWidth
                  error={!!errors.currency}
                  helperText={errors.currency?.message ?? 'Prices and Orders use it. It cannot be changed later.'}
                >
                  {currencyOptions.map((option) => (
                    <MenuItem key={option.code} value={option.code}>
                      {option.label}
                    </MenuItem>
                  ))}
                </TextField>
              )}
            />

            <Button
              type="submit"
              variant="contained"
              size="large"
              disabled={createStore.isPending || availability?.available === false}
            >
              {createStore.isPending ? 'Creating Store…' : 'Create Store'}
            </Button>
          </Stack>
        </Box>
      </Paper>
    </Box>
  )
}
