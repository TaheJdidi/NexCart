import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { Link as RouterLink, useLocation, useNavigate } from 'react-router-dom'
import { Alert, Box, Button, Link, Stack, TextField, Typography } from '@mui/material'
import toast from 'react-hot-toast'
import { toApiError } from '../../lib/apiClient'
import { AuthCard } from './AuthCard'
import { loginSchema, type LoginFormValues } from './schemas'
import { useLogin } from './useAuth'

export function LoginPage() {
  const navigate = useNavigate()
  const location = useLocation()
  const login = useLogin()
  const [formError, setFormError] = useState<string | null>(null)

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: { email: '', password: '' },
  })

  // Return to the page that required sign-in, if any
  const redirectTo = (location.state as { from?: string } | null)?.from ?? '/'

  const onSubmit = handleSubmit(async (values) => {
    setFormError(null)
    try {
      const user = await login.mutateAsync(values)
      toast.success(`Welcome back, ${user.firstName}!`)
      navigate(redirectTo, { replace: true })
    } catch (error) {
      setFormError(toApiError(error).message)
    }
  })

  const { ref: emailRef, ...emailField } = register('email')
  const { ref: passwordRef, ...passwordField } = register('password')

  return (
    <AuthCard title="Log in" subtitle="Welcome back to NexCart">
      <Box component="form" onSubmit={onSubmit} noValidate>
        <Stack spacing={2}>
          {formError && <Alert severity="error">{formError}</Alert>}

          <TextField
            {...emailField}
            inputRef={emailRef}
            label="Email"
            type="email"
            autoComplete="email"
            autoFocus
            fullWidth
            error={!!errors.email}
            helperText={errors.email?.message}
          />
          <TextField
            {...passwordField}
            inputRef={passwordRef}
            label="Password"
            type="password"
            autoComplete="current-password"
            fullWidth
            error={!!errors.password}
            helperText={errors.password?.message}
          />

          <Button type="submit" variant="contained" size="large" disabled={login.isPending}>
            {login.isPending ? 'Logging in…' : 'Log in'}
          </Button>

          <Typography variant="body2" sx={{ textAlign: 'center' }}>
            New to NexCart?{' '}
            <Link component={RouterLink} to="/register" state={location.state}>
              Create an account
            </Link>
          </Typography>
        </Stack>
      </Box>
    </AuthCard>
  )
}
