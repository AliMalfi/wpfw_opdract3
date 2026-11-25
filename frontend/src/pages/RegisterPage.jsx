import { useState } from 'react'
import { Link } from 'react-router-dom'
import { register } from '../api/auth'
import { foutLijst } from '../api/client'

export default function RegisterPage() {
  // 1. State: invoer, fouten en succes
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [herhaal, setHerhaal] = useState('')
  const [fouten, setFouten] = useState([])
  const [succes, setSucces] = useState('')
  const [busy, setBusy] = useState(false)

  async function handleSubmit(e) {
    e.preventDefault()
    setFouten([])
    setSucces('')

    // 2. Eerst zelf controleren
    const lijst = []
    if (!email.trim()) lijst.push('Vul een e-mailadres in.')
    if (password.length < 6) lijst.push('Het wachtwoord moet minimaal 6 tekens hebben.')
    if (password !== herhaal) lijst.push('De wachtwoorden zijn niet hetzelfde.')
    if (lijst.length > 0) {
      setFouten(lijst)
      return
    }

    // 3. Naar de API sturen
    setBusy(true)
    try {
      const result = await register({ email, password })
      setSucces(`Account aangemaakt voor ${result.email} met de rol ${result.rol}. Je kunt nu inloggen.`)
      setPassword('') // 4. Wachtwoord meteen uit het geheugen halen
      setHerhaal('')
    } catch (err) {
      // 5. Fouten van de API tonen (bijv. e-mail bestaat al, wachtwoord te zwak)
      const apiFouten = foutLijst(err)
      setFouten(apiFouten.length > 0 ? apiFouten : [err.message])
    } finally {
      setBusy(false)
    }
  }

  return (
    <section>
      <h1>Registreren</h1>
      <p>Een nieuw account krijgt altijd de rol <strong>Gast</strong>.</p>

      <form onSubmit={handleSubmit} noValidate>
        <div>
          <label htmlFor="reg-email">E-mailadres</label>
          <input
            id="reg-email"
            type="email"
            autoComplete="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </div>
        <div>
          <label htmlFor="reg-password">Wachtwoord</label>
          <input
            id="reg-password"
            type="password"
            autoComplete="new-password"
            aria-describedby="reg-password-hint"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
          <small id="reg-password-hint">
            Minimaal 6 tekens, met een hoofdletter, een kleine letter, een cijfer en een speciaal teken.
          </small>
        </div>
        <div>
          <label htmlFor="reg-herhaal">Herhaal wachtwoord</label>
          <input
            id="reg-herhaal"
            type="password"
            autoComplete="new-password"
            value={herhaal}
            onChange={(e) => setHerhaal(e.target.value)}
            required
          />
        </div>

        {/* 6. Fouten als lijst */}
        {fouten.length > 0 && (
          <div role="alert">
            <p>Dit ging niet goed:</p>
            <ul>
              {fouten.map((f) => <li key={f}>{f}</li>)}
            </ul>
          </div>
        )}

        {/* 7. Succesmelding */}
        {succes && (
          <p role="status">
            {succes} <Link to="/inloggen">Naar inloggen</Link>
          </p>
        )}

        <button type="submit" disabled={busy}>
          {busy ? 'Bezig…' : 'Account maken'}
        </button>
      </form>
    </section>
  )
}