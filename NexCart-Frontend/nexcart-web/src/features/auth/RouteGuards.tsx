import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { Box, CircularProgress } from '@mui/material'
import { useCurrentUser } from './useAuth'

function FullPageSpinner() {
  return (
    <Box sx={{ display: 'flex', justifyContent: 'center', py: 10 }}>
      <CircularProgress aria-label="Loading" />
    </Box>
  )
}

/** Renders child routes only for signed-in users; everyone else goes to /login. */
export function RequireAuth() {
  const { data: user, isPending } = useCurrentUser()
  const location = useLocation()

  if (isPending) return <FullPageSpinner />
  if (!user) return <Navigate to="/login" replace state={{ from: location.pathname }} />
  return <Outlet />
}

/** Login and sign-up pages: signed-in users are sent to the home page instead. */
export function GuestOnly() {
  const { data: user, isPending } = useCurrentUser()

  if (isPending) return <FullPageSpinner />
  if (user) return <Navigate to="/" replace />
  return <Outlet />
}
