import { Link as RouterLink } from 'react-router-dom'
import { Button, Stack, Typography } from '@mui/material'
import { useCurrentUser } from '../features/auth/useAuth'

export function HomePage() {
  const { data: user } = useCurrentUser()

  return (
    <Stack spacing={2} sx={{ py: 6, alignItems: 'flex-start' }}>
      <Typography component="h1" variant="h4" sx={{ fontWeight: 700 }}>
        {user ? `Welcome, ${user.firstName}!` : 'Welcome to NexCart'}
      </Typography>
      <Typography color="text.secondary">
        {user ? `You are signed in as ${user.email}.` : 'Sign in or create an account to get started.'}
      </Typography>
      {!user && (
        <Button component={RouterLink} to="/register" variant="contained">
          Create an account
        </Button>
      )}
    </Stack>
  )
}
