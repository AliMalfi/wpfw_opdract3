import { Link } from 'react-router-dom'

// 1. Dit component krijgt één project binnen via props
export default function ProjectCard({ project }) {
  return (
    <article>
      {/* 2. Titel is een link naar de detailpagina */}
      <h2>
        <Link to={`/projecten/${project.id}`}>{project.titel}</Link>
      </h2>
      {/* 3. Extra info tonen */}
      <p>{project.categorie}</p>
    </article>
  )
}