import { apiRequest, ApiError } from './client'

// 1. Alle blogposts
export const getBlogposts = (signal) => apiRequest('/Blogposts', { signal })

// 2. Eén blogpost; 404 → null
export async function getBlogpostById(id, signal) {
  try {
    return await apiRequest(`/Blogposts/${id}`, { signal })
  } catch (err) {
    if (err instanceof ApiError && err.status === 404) return null
    throw err
  }
}

// 3. Schrijven (voor straks)
export const createBlogpost = (data) =>
  apiRequest('/Blogposts', { method: 'POST', body: data })

export const updateBlogpost = (id, data) =>
  apiRequest(`/Blogposts/${id}`, { method: 'PUT', body: data })

export const deleteBlogpost = (id) =>
  apiRequest(`/Blogposts/${id}`, { method: 'DELETE' })