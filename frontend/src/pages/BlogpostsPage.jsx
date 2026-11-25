import { useEffect, useState } from 'react'
import { getBlogposts } from '../api/blogposts'
import BlogpostCard from '../components/BlogpostCard'

export default function BlogpostsPage() {
  // 1. State: data, laden en fout
  const [blogposts, setBlogposts] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  // 2. Bij openen: blogposts ophalen
  useEffect(() => {
    getBlogposts()
      .then((data) => setBlogposts(data))
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false))
  }, [])

  // 3. Laden, fout en lege lijst
  if (loading) return <p role="status">Blogposts laden…</p>
  if (error) return <p role="alert">Er ging iets mis: {error}</p>
  if (blogposts.length === 0) return <p>Er zijn nog geen blogposts.</p>

  // 4. De lijst
  return (
    <section>
      <h1>Blog</h1>
      {blogposts.map((blogpost) => (
        <BlogpostCard key={blogpost.id} blogpost={blogpost} />
      ))}
    </section>
  )
}