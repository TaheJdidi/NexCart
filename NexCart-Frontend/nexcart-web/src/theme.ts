import { createTheme } from '@mui/material'

export const theme = createTheme({
  colorSchemes: { light: true, dark: true },
  palette: {
    primary: { main: '#4f46e5' },
  },
  shape: { borderRadius: 10 },
  typography: {
    fontFamily: 'system-ui, "Segoe UI", Roboto, sans-serif',
    button: { textTransform: 'none', fontWeight: 600 },
  },
})
