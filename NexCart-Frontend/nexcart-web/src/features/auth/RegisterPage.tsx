import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { Link as RouterLink, useLocation, useNavigate } from 'react-router-dom'
import { Alert, Box, Button, Link, Stack, TextField, Typography } from '@mui/material'
import toast from 'react-hot-toast'
import { toApiError } from '../../lib/apiClient'
import { AuthCard } from './AuthCard'
import { registerSchema, type RegisterFormValues } from './schemas'
import { useRegister } from './useAuth'

const serverFields = ['firstName', 'lastName', 'email', 'password'] as const

export function RegisterPage() {
  const navigate = useNavigate()
  const location = useLocation()
  const registerUser = useRegister()
  const [formError, setFormError] = useState<string | null>(null)

  const {
    register,
    handleSubmit,
    setError,
    formState: { errors },
  } = useForm<RegisterFormValues>({
    resolver: zodResolver(registerSchema),
    defaultValues: { firstName: '', lastName: '', email: '', password: '', confirmPassword: '' },
  })

  const redirectTo = (location.state as { from?: string } | null)?.from ?? '/'

  const onSubmit = handleSubmit(async ({ confirmPassword: _, ...values }) => {
    setFormError(null)
    try {
      const user = await registerUser.mutateAsync(values)
      toast.success(`Welcome to NexCart, ${user.firstName}!`)
      navigate(redirectTo, { replace: true })
    } catch (error) {
      const apiError = toApiError(error)
      let shownOnField = false

      // Show server-side errors (e.g. "email already taken") next to the matching field
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

  const field = (name: keyof RegisterFormValues) => {
    const { ref, ...rest } = register(name)
    return {
      ...rest,
      inputRef: ref,
      fullWidth: true,
      error: !!errors[name],
      helperText: errors[name]?.message,
    }
  }

  return (
    <AuthCard title="Create your account" subtitle="Sign up to start shopping on NexCart">
      <Box component="form" onSubmit={onSubmit} noValidate>
        <Stack spacing={2}>
          {formError && <Alert severity="error">{formError}</Alert>}

          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
            <TextField {...field('firstName')} label="First name" autoComplete="given-name" autoFocus />
            <TextField {...field('lastName')} label="Last name" autoComplete="family-name" />
          </Stack>
          <TextField {...field('email')} label="Email" type="email" autoComplete="email" />
          <TextField
            {...field('password')}
            label="Password"
            type="password"
            autoComplete="new-password"
            helperText={
              errors.password?.message ?? 'At least 8 characters, with upper and lower case letters, a digit and a symbol'
            }
          />
          <TextField {...field('confirmPassword')} label="Confirm password" type="password" autoComplete="new-password" />

          <Button type="submit" variant="contained" size="large" disabled={registerUser.isPending}>
            {registerUser.isPending ? 'Creating account…' : 'Sign up'}
          </Button>

          <Typography variant="body2" sx={{ textAlign: 'center' }}>
            Already have an account?{' '}
            <Link component={RouterLink} to="/login" state={location.state}>
              Log in
            </Link>
          </Typography>
        </Stack>
      </Box>
    </AuthCard>
  )
}
