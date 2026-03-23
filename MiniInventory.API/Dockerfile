# 1. Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Docker giờ đang đứng ngay cạnh file .csproj rồi
COPY *.csproj ./
RUN dotnet restore

# Copy toàn bộ code
COPY . .
RUN dotnet publish -c Release -o /app/publish

# 2. Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "MiniInventory.API.dll"]