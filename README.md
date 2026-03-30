# Mini Warehouse Dashboard

Full-Stack Warehouse Management Demo mit Blazor, 
ASP.NET Core und PostgreSQL.
Kleines Test Projekt für Blazor

## Tech Stack
- Blazor + MudBlazor (Frontend)
- ASP.NET Core (Backend)  
- PostgreSQL (Datenbank)
- JWT + HTTPOnly Cookie (Auth)
- xUnit + Moq (Tests)

## Starten
docker-compose up

## Architektur
- MiniWarehouse.Api – ASP.NET Core Backend
- MiniWarehouse.Client – Blazor Frontend
- MiniWarehouse.Shared – Gemeinsame Models
- MiniWarehouse.Api.Tests – Unit Tests
- MiniWarehouse.Client.Tests – Component Tests
