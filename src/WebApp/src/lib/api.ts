const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8080'
type TokenPair = { accessToken: string; refreshToken: string; expiresInSeconds: number }
export type ApiContext = { getAccess: () => string | null; getRefresh: () => string | null; setTokens: (t: TokenPair) => void; clear: () => void }
export function createApi(ctx: ApiContext) {
  async function rawFetch(path: string, options: RequestInit = {}, attachAuth = true): Promise<Response> {
    const headers: Record<string, string> = { 'Content-Type': 'application/json', ...(options.headers as Record<string, string> || {}) }
    const token = attachAuth ? ctx.getAccess() : null
    if (token) headers['Authorization'] = `Bearer ${token}`
    const res = await fetch(`${API_URL}${path}`, { ...options, headers })
    return res
  }
  async function fetchWithRefresh(path: string, options: RequestInit = {}): Promise<Response> {
    let res = await rawFetch(path, options, true)
    if (res.status !== 401) return res
    const refresh = ctx.getRefresh()
    if (!refresh) return res
    const r = await rawFetch('/api/v1/auth/refresh', { method: 'POST', body: JSON.stringify({ refreshToken: refresh }) }, false)
    if (!r.ok) { ctx.clear(); return res }
    const tp: TokenPair = await r.json(); ctx.setTokens(tp)
    res = await rawFetch(path, options, true)
    return res
  }
  return { fetch: fetchWithRefresh, raw: rawFetch, API_URL }
}