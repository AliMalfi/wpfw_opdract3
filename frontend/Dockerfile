# 1. Bouwfase: React-productiebuild maken met Node
FROM node:22-alpine AS build
WORKDIR /app
COPY frontend/package*.json ./
RUN npm ci
COPY frontend/ ./
RUN npm run build

# 2. Eindfase: Nginx levert de gebouwde bestanden (map dist)
FROM nginxinc/nginx-unprivileged:stable-alpine
COPY frontend/nginx.conf.template /etc/nginx/templates/default.conf.template
COPY --from=build /app/dist /usr/share/nginx/html
# Standaard adres van de API (binnen Compose); in Azure overschrijven we dit
ENV API_URL=http://api:8080
EXPOSE 8080