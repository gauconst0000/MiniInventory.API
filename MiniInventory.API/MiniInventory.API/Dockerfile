# 1. Build code
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Docker bây giờ đang đứng ở trong thư mục MiniInventory.API rồi, nên gọi trực tiếp file luôn
# LƯU Ý: Em hãy check kỹ xem file của em là .API.csproj hay .Api.csproj nhé!
COPY ["MiniInventory.API.csproj", "./"]
RUN dotnet restore "./MiniInventory.API.csproj"

COPY . .
RUN dotnet publish "MiniInventory.API.csproj" -c Release -o /app/publish

# 2. Chạy app
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "MiniInventory.API.dll"]