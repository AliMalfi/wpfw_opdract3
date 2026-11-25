// 1. Alle projecten ophalen uit de API (via de Vite-proxy)
export async function getProjects() {
  const response = await fetch('/api/Projects')

  // 2. Geen 2xx-status? Dan gooien we een fout
  if (!response.ok) {
    throw new Error(`De server gaf status ${response.status}`)
  }

  // 3. JSON omzetten naar een JavaScript-lijst
  return response.json()
}

// 4. Eén project ophalen op id
//    signal = hiermee kunnen we het verzoek annuleren
export async function getProjectById(id, signal) {
  const response = await fetch(`/api/Projects/${id}`, { signal })

  // 5. 404 betekent: dit id bestaat niet → we geven null terug
  if (response.status === 404) return null

  // 6. Andere fout? Dan gooien we een fout
  if (!response.ok) {
    throw new Error(`De server gaf status ${response.status}`)
  }

  // 7. JSON omzetten naar één project
  return response.json()
}