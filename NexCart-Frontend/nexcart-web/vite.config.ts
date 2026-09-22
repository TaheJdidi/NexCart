import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    // Forward API calls to the ASP.NET Core backend, so the browser sees a single origin
    // and the httpOnly auth cookie works without CORS
    proxy: {
      '/api': 'http://localhost:5296',
    },
  },
})
