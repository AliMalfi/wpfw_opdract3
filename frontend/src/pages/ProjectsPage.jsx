import { useEffect, useState } from 'react'
import { getProjects, createProject, updateProject, deleteProject } from '../api/projects'
import { foutLijst } from '../api/client'
import { useAuth } from '../auth/AuthContext'
import ProjectCard from '../components/ProjectCard'
import ProjectForm from '../components/ProjectForm'

export default function ProjectsPage() {
  const { canEdit } = useAuth()

  // 1. State voor de lijst
  const [projects, setProjects] = useState([])
  const [status, setStatus] = useState('loading') // loading | success | error
  const [error, setError] = useState('')

  // 2. State voor acties
  const [editing, setEditing] = useState(undefined) // undefined = dicht, null = nieuw, project = wijzigen
  const [busy, setBusy] = useState(false)
  const [message, setMessage] = useState('')
  const [actieFout, setActieFout] = useState('')
  const [formFouten, setFormFouten] = useState([])

  // 3. Lijst ophalen (ook na opslaan of verwijderen)
  async function loadProjects(signal) {
    const data = await getProjects(signal)
    if (signal?.aborted) return
    setProjects(data)
    setStatus('success')
  }

  // 4. Bij openen van de pagina
  useEffect(() => {
    const controller = new AbortController()
    loadProjects(controller.signal).catch((err) => {
      if (err.name !== 'AbortError') {
        setError(err.message)
        setStatus('error')
      }
    })
    return () => controller.abort()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  // 5. Formulier openen
  function openForm(project) {
    setFormFouten([])
    setMessage('')
    setActieFout('')
    setEditing(project) // null = nieuw
  }

  // 6. Opslaan: POST (nieuw) of PUT (wijzigen)
  async function handleSave(data) {
    setBusy(true)
    setFormFouten([])
    try {
      if (editing) {
        await updateProject(editing.id, data)
        setMessage(`Project "${data.titel}" is gewijzigd.`)
      } else {
        await createProject(data)
        setMessage(`Project "${data.titel}" is toegevoegd.`)
      }
      setEditing(undefined)
      await loadProjects()
    } catch (err) {
      // 400 → validatiefouten van de API; 401/403 → nette tekst uit client.js
      const lijst = foutLijst(err)
      setFormFouten(lijst.length > 0 ? lijst : [err.message])
    } finally {
      setBusy(false)
    }
  }

  // 7. Verwijderen, met bevestiging
  async function handleDelete(project) {
    if (!window.confirm(`Weet je zeker dat je "${project.titel}" wilt verwijderen?`)) return
    setBusy(true)
    setMessage('')
    setActieFout('')
    try {
      await deleteProject(project.id)
      setMessage(`Project "${project.titel}" is verwijderd.`)
      await loadProjects()
    } catch (err) {
      setActieFout(err.message)
    } finally {
      setBusy(false)
    }
  }

  // 8. Laden en fout bij ophalen
  if (status === 'loading') return <p role="status">Projecten laden…</p>
  if (status === 'error') return <p role="alert">Er ging iets mis: {error}</p>

  return (
    <section>
      <h1>Projecten</h1>

      {/* 9. Uitkomst van een actie */}
      {message && <p role="status">{message}</p>}
      {actieFout && <p role="alert">{actieFout}</p>}

      {/* 10. Knop "Nieuw project" alleen voor Editor/Admin */}
      {canEdit && editing === undefined && (
        <button type="button" onClick={() => openForm(null)}>Nieuw project</button>
      )}

      {/* 11. Formulier (nieuw of wijzigen) */}
      {editing !== undefined && (
        <ProjectForm
          key={editing?.id ?? 'nieuw'}
          project={editing}
          busy={busy}
          fouten={formFouten}
          onSave={handleSave}
          onCancel={() => setEditing(undefined)}
        />
      )}

      {/* 12. De lijst (of een melding als hij leeg is) */}
      {projects.length === 0 ? (
        <p>Er zijn nog geen projecten.</p>
      ) : (
        projects.map((project) => (
          <ProjectCard
            key={project.id}
            project={project}
            canEdit={canEdit}
            busy={busy}
            onEdit={() => openForm(project)}
            onDelete={() => handleDelete(project)}
          />
        ))
      )}
    </section>
  )
}