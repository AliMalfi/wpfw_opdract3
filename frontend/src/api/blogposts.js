// 1. Alle blogposts ophalen uit de API
export async function getBlogposts() {
  const response = await fetch('/api/Blogposts')

  // 2. Geen 2xx-status? Dan gooien we een fout
  if (!response.ok) {
    throw new Error(`De server gaf status ${response.status}`)
  }

  return response.json()
}

// 3. Eén blogpost ophalen op id (met annuleer-signaal)
export async function getBlogpostById(id, signal) {
  const response = await fetch(`/api/Blogposts/${id}`, { signal })

  // 4. 404 = bestaat niet → null
  if (response.status === 404) return null

  if (!response.ok) {
    throw new Error(`De server gaf status ${response.status}`)
  }

  return response.json()
}