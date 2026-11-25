import { Link } from 'react-router-dom'

// 1. Wordt getoond als de URL niet bestaat
export default function NotFoundPage() {
  return (
    <section>
      <h1>Pagina niet gevonden</h1>
      <p>Deze pagina bestaat niet.</p>
      <Link to="/">Terug naar home</Link>
    </section>
  )
}