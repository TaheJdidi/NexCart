import { Link as RouterLink } from 'react-router-dom'
import { Button, Card, CardContent, Stack, Typography } from '@mui/material'
import { useCurrentUser } from '../auth/useAuth'
import { storeAddress } from './storeAddress'

export function MyStoresPage() {
  // Rendered under <RequireAuth />, so the user is already loaded
  const { data: user } = useCurrentUser()
  const stores = user?.stores ?? []

  return (
    <Stack spacing={3}>
      <Stack direction="row" sx={{ alignItems: 'center', justifyContent: 'space-between', gap: 2, flexWrap: 'wrap' }}>
        <Typography component="h1" variant="h4" sx={{ fontWeight: 700 }}>
          My Stores
        </Typography>
        {stores.length > 0 && (
          <Button component={RouterLink} to="/stores/new" variant="contained">
            Create a Store
          </Button>
        )}
      </Stack>

      {stores.length === 0 ? (
        <Stack spacing={2} sx={{ py: 6, alignItems: 'flex-start' }}>
          <Typography color="text.secondary">You don{'’'}t have a Store yet.</Typography>
          <Button component={RouterLink} to="/stores/new" variant="contained">
            Create your first Store
          </Button>
        </Stack>
      ) : (
        <Stack spacing={2}>
          {stores.map((store) => (
            <Card key={store.id} variant="outlined">
              <CardContent>
                <Typography variant="h6" component="h2" sx={{ fontWeight: 600 }}>
                  {store.name}
                </Typography>
                <Typography color="text.secondary" sx={{ wordBreak: 'break-all' }}>
                  {storeAddress(store.handle)}
                </Typography>
              </CardContent>
            </Card>
          ))}
        </Stack>
      )}
    </Stack>
  )
}
