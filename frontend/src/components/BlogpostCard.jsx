import { Link } from 'react-router-dom'

// 1. Eén blogpost in de lijst, via props
export default function BlogpostCard({ blogpost }) {
  return (
    <article>
      {/* 2. Titel als link naar de detailpagina */}
      <h2>
        <Link to={`/blog/${blogpost.id}`}>{blogpost.titel}</Link>
      </h2>
      {/* 3. Publicatiedatum, netjes in Nederlands formaat */}
      <p>
        <time dateTime={blogpost.publicatiedatum}>
          {new Date(blogpost.publicatiedatum).toLocaleDateString('nl-NL')}
        </time>
      </p>
    </article>
  )
}