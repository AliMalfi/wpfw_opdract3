import { Link } from 'react-router-dom'

// 1. Eén project; met beheerknoppen als canEdit true is
export default function ProjectCard({ project, canEdit, busy, onEdit, onDelete }) {
  return (
    <article>
      <h2>
        <Link to={`/projecten/${project.id}`}>{project.titel}</Link>
      </h2>
      <p>{project.categorie}</p>

      {/* 2. Alleen zichtbaar voor Editor of Admin */}
      {canEdit && (
        <p>
          <button type="button" onClick={onEdit} disabled={busy}>Wijzigen</button>{' '}
          <button type="button" onClick={onDelete} disabled={busy}>Verwijderen</button>
        </p>
      )}
    </article>
  )
}