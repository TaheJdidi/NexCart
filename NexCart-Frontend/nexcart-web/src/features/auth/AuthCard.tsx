import type { ReactNode } from 'react'
import { Box, Paper, Typography } from '@mui/material'

interface AuthCardProps {
  title: string
  subtitle: string
  children: ReactNode
}

/** Centered card shared by the login and sign-up pages. */
export function AuthCard({ title, subtitle, children }: AuthCardProps) {
  return (
    <Box sx={{ display: 'flex', justifyContent: 'center', py: { xs: 4, sm: 8 } }}>
      <Paper variant="outlined" sx={{ p: { xs: 3, sm: 4 }, width: '100%', maxWidth: 440 }}>
        <Typography component="h1" variant="h5" sx={{ fontWeight: 600 }}>
          {title}
        </Typography>
        <Typography color="text.secondary" sx={{ mt: 0.5, mb: 3 }}>
          {subtitle}
        </Typography>
        {children}
      </Paper>
    </Box>
  )
}
