import { Routes, Route, NavLink } from 'react-router-dom'
import { useAuth } from './auth/AuthContext'
import HomePage from './pages/HomePage'
import ProjectsPage from './pages/ProjectsPage'
import ProjectDetailPage from './pages/ProjectDetailPage'
import BlogpostsPage from './pages/BlogpostsPage'
import BlogpostDetailPage from './pages/BlogpostDetailPage'
import NotFoundPage from './pages/NotFoundPage'

export default function App() {
  // 1. Wie is er ingelogd? (komt van het prikbord)
  const { user, loading, logout } = useAuth()

  return (
    <>
      <header>
        {/* 2. Hoofdmenu */}
        <nav aria-label="Hoofdnavigatie">
          <NavLink to="/">Home</NavLink> |{' '}
          <NavLink to="/projecten">Projecten</NavLink> |{' '}
          <NavLink to="/blog">Blog</NavLink>
        </nav>

        {/* 3. Aanmeldstatus */}
        <p role="status">
          {loading && 'Aanmeldstatus controleren…'}
          {!loading && user && (
            <>
              Ingelogd als <strong>{user.email}</strong> ({user.rollen.join(', ')}){' '}
              <button type="button" onClick={logout}>Uitloggen</button>
            </>
          )}
          {!loading && !user && 'Niet ingelogd'}
        </p>
      </header>

      {/* 4. De pagina die bij de URL hoort */}
      <main>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/projecten" element={<ProjectsPage />} />
          <Route path="/projecten/:id" element={<ProjectDetailPage />} />
          <Route path="/blog" element={<BlogpostsPage />} />
          <Route path="/blog/:id" element={<BlogpostDetailPage />} />
          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </main>
    </>
  )
}