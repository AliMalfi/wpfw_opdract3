import { useEffect, useState } from 'react'
import { useParams, Link } from 'react-router-dom'
import { getProjectById } from '../api/projects'

export default function ProjectDetailPage() {
  // 1. Het id uit de URL halen, bijvoorbeeld /projecten/3 → "3"
  const { id } = useParams()

  // 2. Het resultaat onthouden, samen met het id waar het bij hoort
  const [result, setResult] = useState({ id: null, project: null, error: null })

  useEffect(() => {
    // 3. Een "annuleerknop" voor dit verzoek
    const controller = new AbortController()

    getProjectById(id, controller.signal)
      .then((project) => setResult({ id, project, error: null }))
      .catch((err) => {
        // 4. Een geannuleerd verzoek is geen echte fout
        if (err.name !== 'AbortError') {
          setResult({ id, project: null, error: err.message })
        }
      })

    // 5. Verandert het id? Dan wordt het oude verzoek geannuleerd
    return () => controller.abort()
  }, [id])

  // 6. Laden zolang het resultaat nog niet bij het id uit de URL hoort
  const loading = result.id !== id

  if (loading) return <p role="status">Project laden…</p>
  if (result.error) return <p role="alert">Er ging iets mis: {result.error}</p>
  if (!result.project) {
    return (
      <section>
        <h1>Project niet gevonden</h1>
        <p>Er bestaat geen project met id {id}.</p>
        <Link to="/projecten">Terug naar projecten</Link>
      </section>
    )
  }

  // 7. Het project tonen
  const project = result.project
  return (
    <article>
      <h1>{project.titel}</h1>
      <p>{project.beschrijving}</p>
      <dl>
        <dt>Categorie</dt>
        <dd>{project.categorie}</dd>
        <dt>Datum</dt>
        <dd>{new Date(project.datum).toLocaleDateString('nl-NL')}</dd>
      </dl>
      {project.githubUrl && (
        <p>
          <a href={project.githubUrl} target="_blank" rel="noreferrer">
            Bekijk op GitHub
          </a>
        </p>
      )}
      <Link to="/projecten">← Terug naar projecten</Link>
    </article>
  )
}