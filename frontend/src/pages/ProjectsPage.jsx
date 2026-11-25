import { useEffect, useState } from 'react'
import { getProjects } from '../api/projects'
import ProjectCard from '../components/ProjectCard'

export default function ProjectsPage() {
  // 1. State: de data en de toestand van het scherm
  const [projects, setProjects] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  // 2. Bij het openen van de pagina: projecten ophalen
  useEffect(() => {
    getProjects()
      .then((data) => setProjects(data))
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false))
  }, [])

  // 3. Laden, fout en lege lijst apart tonen
  if (loading) return <p role="status">Projecten laden…</p>
  if (error) return <p role="alert">Er ging iets mis: {error}</p>
  if (projects.length === 0) return <p>Er zijn nog geen projecten.</p>

  // 4. De lijst: elk project wordt een ProjectCard
  return (
    <section>
      <h1>Projecten</h1>
      {projects.map((project) => (
        <ProjectCard key={project.id} project={project} />
      ))}
    </section>
  )
}