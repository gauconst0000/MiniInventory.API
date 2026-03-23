# 1. Build Angular
FROM node:20 AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build --configuration=production

# 2. Dùng Nginx để "phát" trang web lên
FROM nginx:alpine
# Lưu ý: 'mini-inventory-ui' là tên folder trong dist sau khi build
COPY --from=build /app/dist/mini-inventory-ui/browser /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]