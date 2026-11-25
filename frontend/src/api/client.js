// 1. Nette teksten per HTTP-foutcode (voor de gebruiker)
const foutTeksten = {
  400: 'De invoer is niet geldig.',
  401: 'Je bent niet ingelogd. Log eerst in.',
  403: 'Je hebt geen rechten voor deze actie. Alleen Editor of Admin mag dit.',
  404: 'Niet gevonden.',
  500: 'Er ging iets mis op de server.',
}

// 2. Eigen fouttype: bewaart de status (bijv. 401) en eventuele details van de API
export class ApiError extends Error {
  constructor(status, message, details) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.details = details
  }
}

// 3. Eén functie voor álle verzoeken naar de API
export async function apiRequest(path, { body, ...options } = {}) {
  const response = await fetch(`/api${path}`, {
    ...options,
    credentials: 'include', // 4. cookie altijd meesturen
    headers: {
      // 5. Alleen bij een body: zeggen dat het JSON is
      ...(body !== undefined ? { 'Content-Type': 'application/json' } : {}),
      ...options.headers,
    },
    // 6. JavaScript-object omzetten naar JSON-tekst
    ...(body !== undefined ? { body: JSON.stringify(body) } : {}),
  })

  // 7. Fout (geen 2xx)? Dan een ApiError gooien met een nette tekst
  if (!response.ok) {
    let details = null
    try {
      details = await response.json() // bijv. validatiefouten bij 400
    } catch {
      // geen JSON in het antwoord (bijv. bij 401 of 403)
    }
    throw new ApiError(
      response.status,
      foutTeksten[response.status] ?? `Onbekende fout (${response.status})`,
      details
    )
  }

  // 8. 204 No Content of leeg antwoord → null, anders JSON
  if (response.status === 204) return null
  const tekst = await response.text()
  return tekst ? JSON.parse(tekst) : null
}

// 9. Losse foutteksten uit een ApiError halen (voor formulieren)
//    - Identity (register): een lijst met { description }
//    - Validatie ([ApiController]): { errors: { Veld: ["tekst"] } }
export function foutLijst(err) {
  const details = err?.details
  if (Array.isArray(details)) return details.map((d) => d.description)
  if (details?.errors) return Object.values(details.errors).flat()
  return []
}