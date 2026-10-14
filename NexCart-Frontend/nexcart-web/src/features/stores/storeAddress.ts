/** The root domain Store sites live under; defaults to the current host (e.g. localhost in development). */
export const platformDomain = import.meta.env.VITE_PLATFORM_DOMAIN ?? window.location.hostname

/** The address of a Store's site, which is its Store Handle as a subdomain. */
export function storeAddress(handle: string) {
  return `${handle}.${platformDomain}`
}
