import axios from 'axios'

/** Axios instance for the NexCart API. The auth cookie is sent automatically (same origin). */
export const apiClient = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' },
})

/** Error body returned by the backend's exception middleware. */
interface ProblemDetails {
  title?: string
  detail?: string
  status?: number
  errors?: Record<string, string[]> | null
}

export interface ApiError {
  status?: number
  message: string
  fieldErrors: Record<string, string>
}

/** Turns any thrown value into a message and a map of field name -> first error. */
export function toApiError(error: unknown): ApiError {
  if (axios.isAxiosError<ProblemDetails>(error)) {
    const data = error.response?.data
    const fieldErrors: Record<string, string> = {}

    for (const [field, messages] of Object.entries(data?.errors ?? {})) {
      if (messages.length > 0) {
        fieldErrors[field] = messages[0]
      }
    }

    return {
      status: error.response?.status,
      message: data?.detail ?? data?.title ?? (error.response ? 'Something went wrong.' : 'Cannot reach the server.'),
      fieldErrors,
    }
  }

  return { message: 'Something went wrong.', fieldErrors: {} }
}
