import { useEffect, useState } from 'react'
import { useParams, Link } from 'react-router-dom'
import { getBlogpostById } from '../api/blogposts'

export default function BlogpostDetailPage() {
  // 1. Id uit de URL, bijvoorbeeld /blog/2 → "2"
  const { id } = useParams()

  // 2. Resultaat + het id waar het bij hoort
  const [result, setResult] = useState({ id: null, blogpost: null, error: null })

  useEffect(() => {
    // 3. Annuleerknop voor dit verzoek
    const controller = new AbortController()

    getBlogpostById(id, controller.signal)
      .then((blogpost) => setResult({ id, blogpost, error: null }))
      .catch((err) => {
        if (err.name !== 'AbortError') {
          setResult({ id, blogpost: null, error: err.message })
        }
      })

    // 4. Nieuw id? Oud verzoek annuleren
    return () => controller.abort()
  }, [id])

  // 5. Laden zolang het resultaat niet bij dit id hoort
  const loading = result.id !== id

  if (loading) return <p role="status">Blogpost laden…</p>
  if (result.error) return <p role="alert">Er ging iets mis: {result.error}</p>
  if (!result.blogpost) {
    return (
      <section>
        <h1>Blogpost niet gevonden</h1>
        <p>Er bestaat geen blogpost met id {id}.</p>
        <Link to="/blog">Terug naar de blog</Link>
      </section>
    )
  }

  // 6. De blogpost tonen
  const blogpost = result.blogpost
  return (
    <article>
      <h1>{blogpost.titel}</h1>
      <p>
        <time dateTime={blogpost.publicatiedatum}>
          {new Date(blogpost.publicatiedatum).toLocaleDateString('nl-NL')}
        </time>
      </p>
      <p>{blogpost.inhoud}</p>
      <Link to="/blog">← Terug naar de blog</Link>
    </article>
  )
}