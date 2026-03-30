# 1. Build-Stage (SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Projektdaten kopieren für Cache
COPY ["MiniWarehouse.Client/MiniWarehouse.Client.csproj", "MiniWarehouse.Client/"]
COPY ["MiniWarehouse.Shared/MiniWarehouse.Shared.csproj", "MiniWarehouse.Shared/"]
RUN dotnet restore "MiniWarehouse.Client/MiniWarehouse.Client.csproj"

# Alles kopieren und die statischen Dateien generieren
COPY . .
WORKDIR "/src/MiniWarehouse.Client"
RUN dotnet publish "MiniWarehouse.Client.csproj" -c Release -o /app/publish

# 2. Final-Stage (Nginx - Der Webserver für statische Dateien)
# ... (Build-Schritte wie vorher)

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
# Kopiere deine eigene Config in den Container
COPY MiniWarehouse.Client/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/publish/wwwroot .
EXPOSE 80