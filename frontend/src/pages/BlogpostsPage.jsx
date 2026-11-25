import { useEffect, useState } from 'react'
import { getBlogposts, createBlogpost, updateBlogpost, deleteBlogpost } from '../api/blogposts'
import { foutLijst } from '../api/client'
import { useAuth } from '../auth/AuthContext'
import BlogpostCard from '../components/BlogpostCard'
import BlogpostForm from '../components/BlogpostForm'

export default function BlogpostsPage() {
  const { canEdit } = useAuth()

  // 1. State voor de lijst
  const [blogposts, setBlogposts] = useState([])
  const [status, setStatus] = useState('loading') // loading | success | error
  const [error, setError] = useState('')

  // 2. State voor acties
  const [editing, setEditing] = useState(undefined) // undefined = dicht, null = nieuw, blogpost = wijzigen
  const [busy, setBusy] = useState(false)
  const [message, setMessage] = useState('')
  const [actieFout, setActieFout] = useState('')
  const [formFouten, setFormFouten] = useState([])

  // 3. Lijst opnieuw ophalen (na opslaan of verwijderen)
  async function loadBlogposts() {
    const data = await getBlogposts()
    setBlogposts(data)
    setStatus('success')
  }

  // 4. Bij openen van de pagina: lijst ophalen
  useEffect(() => {
    const controller = new AbortController()
    getBlogposts(controller.signal)
      .then((data) => {
        setBlogposts(data)
        setStatus('success')
      })
      .catch((err) => {
        if (err.name !== 'AbortError') {
          setError(err.message)
          setStatus('error')
        }
      })
    // 5. Pagina verlaten? Verzoek annuleren
    return () => controller.abort()
  }, [])

  // 6. Formulier openen (null = nieuw, blogpost = wijzigen)
  function openForm(blogpost) {
    setFormFouten([])
    setMessage('')
    setActieFout('')
    setEditing(blogpost)
  }

  // 7. Opslaan: POST (nieuw) of PUT (wijzigen)
  async function handleSave(data) {
    setBusy(true)
    setFormFouten([])
    try {
      if (editing) {
        await updateBlogpost(editing.id, data)
        setMessage(`Blogpost "${data.titel}" is gewijzigd.`)
      } else {
        await createBlogpost(data)
        setMessage(`Blogpost "${data.titel}" is toegevoegd.`)
      }
      setEditing(undefined)
      await loadBlogposts()
    } catch (err) {
      const lijst = foutLijst(err)
      setFormFouten(lijst.length > 0 ? lijst : [err.message])
    } finally {
      setBusy(false)
    }
  }

  // 8. Verwijderen, met bevestiging
  async function handleDelete(blogpost) {
    if (!window.confirm(`Weet je zeker dat je "${blogpost.titel}" wilt verwijderen?`)) return
    setBusy(true)
    setMessage('')
    setActieFout('')
    try {
      await deleteBlogpost(blogpost.id)
      setMessage(`Blogpost "${blogpost.titel}" is verwijderd.`)
      await loadBlogposts()
    } catch (err) {
      setActieFout(err.message)
    } finally {
      setBusy(false)
    }
  }

  // 9. Laden en fout bij ophalen
  if (status === 'loading') return <p role="status">Blogposts laden…</p>
  if (status === 'error') return <p role="alert">Er ging iets mis: {error}</p>

  return (
    <section>
      <h1>Blog</h1>

      {/* 10. Uitkomst van een actie */}
      {message && <p role="status">{message}</p>}
      {actieFout && <p role="alert">{actieFout}</p>}

      {/* 11. Knop "Nieuwe blogpost" alleen voor Editor/Admin */}
      {canEdit && editing === undefined && (
        <button type="button" onClick={() => openForm(null)}>Nieuwe blogpost</button>
      )}

      {/* 12. Formulier (nieuw of wijzigen) */}
      {editing !== undefined && (
        <BlogpostForm
          key={editing?.id ?? 'nieuw'}
          blogpost={editing}
          busy={busy}
          fouten={formFouten}
          onSave={handleSave}
          onCancel={() => setEditing(undefined)}
        />
      )}

      {/* 13. De lijst (of een melding als hij leeg is) */}
      {blogposts.length === 0 ? (
        <p>Er zijn nog geen blogposts.</p>
      ) : (
        blogposts.map((blogpost) => (
          <BlogpostCard
            key={blogpost.id}
            blogpost={blogpost}
            canEdit={canEdit}
            busy={busy}
            onEdit={() => openForm(blogpost)}
            onDelete={() => handleDelete(blogpost)}
          />
        ))
      )}
    </section>
  )
}