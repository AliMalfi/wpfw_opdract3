import { Link } from 'react-router-dom'

// 1. Eén blogpost; met beheerknoppen als canEdit true is
export default function BlogpostCard({ blogpost, canEdit, busy, onEdit, onDelete }) {
  return (
    <article>
      <h2>
        <Link to={`/blog/${blogpost.id}`}>{blogpost.titel}</Link>
      </h2>

      {/* 2. Publicatiedatum in grijs */}
      <p className="meta">
        <time dateTime={blogpost.publicatiedatum}>
          {new Date(blogpost.publicatiedatum).toLocaleDateString('nl-NL')}
        </time>
      </p>

      {/* 3. Alleen zichtbaar voor Editor of Admin */}
      {canEdit && (
        <p className="acties">
          <button type="button" onClick={onEdit} disabled={busy}>Wijzigen</button>
          <button type="button" className="btn-danger" onClick={onDelete} disabled={busy}>Verwijderen</button>
        </p>
      )}
    </article>
  )
}