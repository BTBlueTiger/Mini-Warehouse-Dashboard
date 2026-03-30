# 1. Runtime-Image (Schlank für die Ausführung)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# Port 8080 ist Standard in .NET 8 Containern, 
# aber wir können ihn auf deinen Wunsch-Port mappen
EXPOSE 8080 

# 2. Build-Image (Enthält das SDK zum Kompilieren)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# --- SCHRITT A: Projektdateien kopieren für Cache-Optimierung ---
# Wir kopieren NUR die .csproj Dateien. 
# Wenn sich diese nicht ändern, überspringt Docker das 'dotnet restore' beim nächsten Mal.
COPY ["MiniWarehouse.Api/MiniWarehouse.Api.csproj", "MiniWarehouse.Api/"]
COPY ["MiniWarehouse.Shared/MiniWarehouse.Shared.csproj", "MiniWarehouse.Shared/"]

# Abhängigkeiten wiederherstellen
RUN dotnet restore "MiniWarehouse.Api/MiniWarehouse.Api.csproj"

# --- SCHRITT B: Den Rest kopieren und bauen ---
# Jetzt kopieren wir den tatsächlichen Quellcode
COPY . .
WORKDIR "/src/MiniWarehouse.Api"
RUN dotnet build "MiniWarehouse.Api.csproj" -c Release -o /app/build

# 3. Publish-Schritt
FROM build AS publish
RUN dotnet publish "MiniWarehouse.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Finales Image (Nur die fertigen Dateien, ohne SDK)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MiniWarehouse.Api.dll"]