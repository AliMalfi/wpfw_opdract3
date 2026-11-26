import { Routes, Route, NavLink, Link } from 'react-router-dom'
import { useAuth } from './auth/AuthContext'
import HomePage from './pages/HomePage'
import ProjectsPage from './pages/ProjectsPage'
import ProjectDetailPage from './pages/ProjectDetailPage'
import BlogpostsPage from './pages/BlogpostsPage'
import BlogpostDetailPage from './pages/BlogpostDetailPage'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import NotFoundPage from './pages/NotFoundPage'

export default function App() {
  // 1. Wie is er ingelogd?
  const { user, loading, logout } = useAuth()

  return (
    <>
      {/* 2. Toetsenbord: direct naar de inhoud springen */}
      <a href="#inhoud" className="skip-link">Ga naar inhoud</a>

      <header className="site-header">
        <div className="header-inner">
          {/* 3. Logo zoals op mijn oude portfolio */}
          <p className="logo">
            <Link to="/"><span>A</span>li <span>M</span>alfi</Link>
          </p>

          {/* 4. Hoofdmenu (NavLink zet zelf class="active" en aria-current) */}
          <nav aria-label="Hoofdnavigatie">
            <ul>
              <li><NavLink to="/" end>Home</NavLink></li>
              <li><NavLink to="/projecten">Projecten</NavLink></li>
              <li><NavLink to="/blog">Blog</NavLink></li>
            </ul>
          </nav>

          {/* 5. Aanmeldstatus */}
          <div className="auth-status" role="status">
            {loading && <span>Aanmeldstatus controleren…</span>}
            {!loading && user && (
              <>
                <span>{user.email}</span>
                <span className="rol">{user.rollen.join(', ')}</span>
                <button type="button" onClick={logout}>Uitloggen</button>
              </>
            )}
            {!loading && !user && (
              <>
                <NavLink to="/inloggen">Inloggen</NavLink>
                <NavLink to="/registreren">Registreren</NavLink>
              </>
            )}
          </div>
        </div>
      </header>

      {/* 6. De pagina die bij de URL hoort */}
      <main id="inhoud">
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/projecten" element={<ProjectsPage />} />
          <Route path="/projecten/:id" element={<ProjectDetailPage />} />
          <Route path="/blog" element={<BlogpostsPage />} />
          <Route path="/blog/:id" element={<BlogpostDetailPage />} />
          <Route path="/inloggen" element={<LoginPage />} />
          <Route path="/registreren" element={<RegisterPage />} />
          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </main>

      <footer>
        <p>&copy; 2026 Ali Malfi · versie 2</p>
      </footer>
    </>
  )
}