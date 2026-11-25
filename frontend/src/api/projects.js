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