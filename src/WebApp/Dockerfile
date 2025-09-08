FROM node:20-alpine AS build
WORKDIR /app
COPY package.json ./
COPY tsconfig.json ./
COPY tailwind.config.ts ./
COPY postcss.config.js ./
COPY vite.config.ts ./
COPY index.html ./
COPY src ./src
ARG API_URL=http://localhost:8080
ENV VITE_API_URL=${API_URL}
RUN npm install
RUN npm ci && npm run build
FROM node:20-alpine
WORKDIR /app
COPY --from=build /app/dist ./dist
EXPOSE 5173
CMD ["npx", "vite", "preview", "--host", "0.0.0.0", "--port", "5173"]