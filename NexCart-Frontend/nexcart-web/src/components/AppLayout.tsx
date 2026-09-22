import { Link as RouterLink, Outlet, useNavigate } from 'react-router-dom'
import { AppBar, Box, Button, Container, Stack, Toolbar, Typography } from '@mui/material'
import toast from 'react-hot-toast'
import { useCurrentUser, useLogout } from '../features/auth/useAuth'

export function AppLayout() {
  const { data: user, isPending } = useCurrentUser()
  const logout = useLogout()
  const navigate = useNavigate()

  const handleLogout = async () => {
    try {
      await logout.mutateAsync()
      toast.success('You have been logged out.')
    } catch {
      // The local session is cleared either way; the cookie simply expires on its own
      toast.error('Could not reach the server, but you have been logged out on this device.')
    }
    navigate('/login')
  }

  return (
    <Box sx={{ minHeight: '100vh', bgcolor: 'background.default' }}>
      <AppBar position="sticky" color="inherit" elevation={0} sx={{ borderBottom: 1, borderColor: 'divider' }}>
        <Container maxWidth="lg">
          <Toolbar disableGutters sx={{ gap: 2 }}>
            <Typography
              component={RouterLink}
              to="/"
              variant="h6"
              sx={{ fontWeight: 700, color: 'primary.main', textDecoration: 'none', flexGrow: 1 }}
            >
              NexCart
            </Typography>

            {!isPending &&
              (user ? (
                <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
                  <Typography sx={{ display: { xs: 'none', sm: 'block' } }}>Hi, {user.firstName}</Typography>
                  <Button variant="outlined" onClick={handleLogout} disabled={logout.isPending}>
                    Log out
                  </Button>
                </Stack>
              ) : (
                <Stack direction="row" spacing={1}>
                  <Button component={RouterLink} to="/login">
                    Log in
                  </Button>
                  <Button component={RouterLink} to="/register" variant="contained">
                    Sign up
                  </Button>
                </Stack>
              ))}
          </Toolbar>
        </Container>
      </AppBar>

      <Container maxWidth="lg" component="main" sx={{ py: 4 }}>
        <Outlet />
      </Container>
    </Box>
  )
}
