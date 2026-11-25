import { apiRequest, ApiError } from './client'

// 1. Alle projecten
export const getProjects = (signal) => apiRequest('/Projects', { signal })

// 2. Eén project; 404 → null (dan toont de pagina "niet gevonden")
export async function getProjectById(id, signal) {
  try {
    return await apiRequest(`/Projects/${id}`, { signal })
  } catch (err) {
    if (err instanceof ApiError && err.status === 404) return null
    throw err
  }
}

// 3. Schrijven (gebruiken we straks bij de formulieren)
export const createProject = (data) =>
  apiRequest('/Projects', { method: 'POST', body: data })

export const updateProject = (id, data) =>
  apiRequest(`/Projects/${id}`, { method: 'PUT', body: data })

export const deleteProject = (id) =>
  apiRequest(`/Projects/${id}`, { method: 'DELETE' })