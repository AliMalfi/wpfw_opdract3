import { Link } from 'react-router-dom'

// 1. Eén project; met beheerknoppen als canEdit true is
export default function ProjectCard({ project, canEdit, busy, onEdit, onDelete }) {
  return (
    <article>
      <h2>
        <Link to={`/projecten/${project.id}`}>{project.titel}</Link>
      </h2>

      {/* 2. Categorie in grijs */}
      <p className="meta">{project.categorie}</p>

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