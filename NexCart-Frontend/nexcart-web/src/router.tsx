import { createBrowserRouter, Navigate } from 'react-router-dom'
import { AppLayout } from './components/AppLayout'
import { LoginPage } from './features/auth/LoginPage'
import { RegisterPage } from './features/auth/RegisterPage'
import { GuestOnly } from './features/auth/RouteGuards'
import { HomePage } from './pages/HomePage'

export const router = createBrowserRouter([
  {
    element: <AppLayout />,
    children: [
      { index: true, element: <HomePage /> },
      {
        element: <GuestOnly />,
        children: [
          { path: 'login', element: <LoginPage /> },
          { path: 'register', element: <RegisterPage /> },
        ],
      },
      // Pages that need a signed-in user go under <RequireAuth /> (e.g. orders, account)
      { path: '*', element: <Navigate to="/" replace /> },
    ],
  },
])
