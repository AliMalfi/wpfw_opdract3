import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/AuthContext'
import { ApiError } from '../api/client'

export default function LoginPage() {
  const { login } = useAuth()
  const navigate = useNavigate()

  // 1. State: invoer, foutmelding en "bezig"
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  async function handleSubmit(e) {
    e.preventDefault() // 2. Pagina niet herladen
    setError('')

    // 3. Eerst zelf controleren (snelle feedback)
    if (!email.trim() || !password) {
      setError('Vul je e-mailadres en wachtwoord in.')
      return
    }

    // 4. Naar de API sturen
    setBusy(true)
    try {
      await login({ email, password })
      navigate('/') // 5. Gelukt → naar home
    } catch (err) {
      // 6. 401 = verkeerde gegevens
      if (err instanceof ApiError && err.status === 401) {
        setError('E-mail of wachtwoord is onjuist.')
      } else {
        setError(err.message)
      }
    } finally {
      setBusy(false)
    }
  }

  return (
    <section>
      <h1>Inloggen</h1>
      <form onSubmit={handleSubmit} noValidate>
        {/* 7. Elk veld heeft een label (toegankelijkheid) */}
        <div>
          <label htmlFor="login-email">E-mailadres</label>
          <input
            id="login-email"
            type="email"
            autoComplete="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </div>
        <div>
          <label htmlFor="login-password">Wachtwoord</label>
          <input
            id="login-password"
            type="password"
            autoComplete="current-password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>

        {/* 8. Foutmelding, voorgelezen door schermlezers */}
        {error && <p role="alert">{error}</p>}

        <button type="submit" disabled={busy}>
          {busy ? 'Bezig met inloggen…' : 'Inloggen'}
        </button>
      </form>
      <p>
        Nog geen account? <Link to="/registreren">Registreren</Link>
      </p>
    </section>
  )
}