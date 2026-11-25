import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  // 1. React-plugin van Vite
  plugins: [react()],

  server: {
    // 2. Proxy: alles wat met /api begint, gaat door naar de backend
    proxy: {
      '/api': {
        target: 'https://localhost:7101', // 3. adres van je API
        changeOrigin: true,               // 4. doe alsof het verzoek van de API zelf komt
        secure: false,                    // 5. accepteer het lokale ontwikkel-certificaat (alleen lokaal!)
      },
    },
  },
})