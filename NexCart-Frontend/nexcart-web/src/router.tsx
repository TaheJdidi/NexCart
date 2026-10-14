import { createBrowserRouter, Navigate } from 'react-router-dom'
import { AppLayout } from './components/AppLayout'
import { LoginPage } from './features/auth/LoginPage'
import { RegisterPage } from './features/auth/RegisterPage'
import { GuestOnly, RequireAuth } from './features/auth/RouteGuards'
import { CreateStorePage } from './features/stores/CreateStorePage'
import { MyStoresPage } from './features/stores/MyStoresPage'
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
      // Pages that need a signed-in user
      {
        element: <RequireAuth />,
        children: [
          { path: 'stores', element: <MyStoresPage /> },
          { path: 'stores/new', element: <CreateStorePage /> },
        ],
      },
      { path: '*', element: <Navigate to="/" replace /> },
    ],
  },
])
