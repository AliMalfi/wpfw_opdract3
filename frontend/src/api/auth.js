import { apiRequest, ApiError } from './client'

// 1. Registreren (nieuwe gebruiker wordt altijd Gast)
export const register = (data) =>
  apiRequest('/Auth/register', { method: 'POST', body: data })

// 2. Inloggen (bij succes zet de API de cookie)
export const login = (data) =>
  apiRequest('/Auth/login', { method: 'POST', body: data })

// 3. Uitloggen (de API haalt de cookie weg)
export const logout = () =>
  apiRequest('/Auth/logout', { method: 'POST' })

// 4. Wie ben ik? 401 betekent: niemand ingelogd → null
export async function getMe() {
  try {
    return await apiRequest('/Auth/me')
  } catch (err) {
    if (err instanceof ApiError && err.status === 401) return null
    throw err
  }
}