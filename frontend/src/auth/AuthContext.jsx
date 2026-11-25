import { createContext, useContext, useEffect, useState } from 'react'
import * as authApi from '../api/auth'

// 1. Het "prikbord" voor de ingelogde gebruiker
const AuthContext = createContext(null)

export function AuthProvider({ children }) {
  // 2. user = { email, rollen } of null (niet ingelogd)
  const [user, setUser] = useState(null)
  const [loading, setLoading] = useState(true)

  // 3. Bij het (her)laden van de app: vraag de API wie er ingelogd is
  useEffect(() => {
    authApi
      .getMe()
      .then((current) => setUser(current))
      .catch(() => setUser(null))
      .finally(() => setLoading(false))
  }, [])

  // 4. Opnieuw ophalen (bijv. na inloggen)
  async function refreshUser() {
    const current = await authApi.getMe()
    setUser(current)
  }

  // 5. Inloggen: eerst de API, daarna /me om de rol te weten
  async function login(credentials) {
    await authApi.login(credentials)
    await refreshUser()
  }

  // 6. Uitloggen: cookie weg en user leeg
  async function logout() {
    await authApi.logout()
    setUser(null)
  }

  // 7. Mag deze gebruiker beheren? Alleen Editor of Admin
  const canEdit =
    user?.rollen.some((rol) => ['Editor', 'Admin'].includes(rol)) ?? false

  // 8. Alles op het prikbord hangen
  return (
    <AuthContext.Provider value={{ user, loading, canEdit, login, logout, refreshUser }}>
      {children}
    </AuthContext.Provider>
  )
}

// 9. Korte manier om het prikbord te lezen: const { user } = useAuth()
// eslint-disable-next-line react-refresh/only-export-components
export function useAuth() {
  return useContext(AuthContext)
}