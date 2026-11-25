import { Routes, Route, NavLink } from 'react-router-dom'
import HomePage from './pages/HomePage'
import ProjectsPage from './pages/ProjectsPage'
import ProjectDetailPage from './pages/ProjectDetailPage'
import NotFoundPage from './pages/NotFoundPage'

export default function App() {
  return (
    <>
      {/* 1. Menu: zichtbaar op elke pagina */}
      <header>
        <nav aria-label="Hoofdnavigatie">
          <NavLink to="/">Home</NavLink> |{' '}
          <NavLink to="/projecten">Projecten</NavLink>
        </nav>
      </header>

      {/* 2. Hier komt de pagina die bij de URL hoort */}
      <main>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/projecten" element={<ProjectsPage />} />
          {/* 3. Detailpagina: :id is het stukje uit de URL */}
          <Route path="/projecten/:id" element={<ProjectDetailPage />} />
          {/* 4. Alle andere URL's: 404-pagina */}
          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </main>
    </>
  )
}