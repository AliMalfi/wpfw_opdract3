import { Routes, Route, NavLink } from 'react-router-dom'
import HomePage from './pages/HomePage'
import ProjectsPage from './pages/ProjectsPage'
import ProjectDetailPage from './pages/ProjectDetailPage'
import BlogpostsPage from './pages/BlogpostsPage'
import BlogpostDetailPage from './pages/BlogpostDetailPage'
import NotFoundPage from './pages/NotFoundPage'

export default function App() {
  return (
    <>
      {/* 1. Menu: zichtbaar op elke pagina */}
      <header>
        <nav aria-label="Hoofdnavigatie">
          <NavLink to="/">Home</NavLink> |{' '}
          <NavLink to="/projecten">Projecten</NavLink> |{' '}
          <NavLink to="/blog">Blog</NavLink>
        </nav>
      </header>

      {/* 2. De pagina die bij de URL hoort */}
      <main>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/projecten" element={<ProjectsPage />} />
          <Route path="/projecten/:id" element={<ProjectDetailPage />} />
          <Route path="/blog" element={<BlogpostsPage />} />
          <Route path="/blog/:id" element={<BlogpostDetailPage />} />
          {/* 3. Alle andere URL's: 404-pagina */}
          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </main>
    </>
  )
}