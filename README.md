# Mini-Warehouse Dashboard (3-Tages-Demo)

**Hinweis:** Dies ist ein reines 3-Tages-Demonstrationsprojekt. Es dient ausschließlich zu Lern- und Präsentationszwecken und enthält bewusst Schwächen, wie z.B. einen hardcodierten JWT-Key und vereinfachte Sicherheitsmechanismen. Für produktive Umgebungen ist es nicht geeignet! 

---

## Überblick

Ein modernes, funktionsfähiges Lagerverwaltungs-Dashboard mit .NET 8, Blazor, SignalR, JWT-Authentifizierung und rollenbasiertem Zugriff.

---

## Features

- **Registrierung & Login:**
	- Nutzer können sich registrieren und einloggen. Die Authentifizierung erfolgt sicher per JWT, das als httpOnly-Cookie gesetzt wird.

- **Rollenbasiertes System:**
	- Es gibt zwei Rollen: _User_ und _Admin_.
		- **Admin** sieht zusätzliche Seiten und kann User verwalten.
		- **User** hat Zugriff auf Standardfunktionen.

- **Live-Dashboard:**
	- Das Dashboard zeigt Lagerbewegungen in Echtzeit per SignalR (Websockets). (Simulation)

- **User-Verwaltung (nur Admin):**
	- Admins können alle Nutzer einsehen und verwalten. (Light)

- **Profilseite:**
	- Jeder Nutzer kann sein Profil einsehen und bearbeiten. Nur das Passwort nicht.

- **Settings:**
	- Nutzer können zwischen Light- und Dark-Mode wechseln.

- **Sichere Passwörter:**
	- Passwörter werden gehasht gespeichert.
	- Admin-User wird beim ersten Start automatisch angelegt.

- **Docker-Ready:**
	- Das Projekt kann komplett mit Docker Compose gestartet werden.

- **Tests:**
	- Es gibt erste Unit- und Integrationstests.

---

## Seitenübersicht

####  Nur eingeloggt erreichbar.
- **/dashboard**  
	Live-Ansicht der Lagerbewegungen (SignalR).
- **/admin/users**  
	Userverwaltung (nur für Admins sichtbar).
- **/profile**  
	Profilseite für den eingeloggten User.
- **/settings**  
	Einstellungen inkl. DarkMode.
####  Nur uneingeloggt erreichbar.
- **/**  
	Login- und Registrierungsseite.

---

## Technischer Überblick

- **Backend:** ASP.NET Core WebAPI, Entity Framework Core, SignalR, JWT
- **Frontend:** Blazor WebAssembly, MudBlazor (UI), rollenbasierte Navigation
- **Datenbank:** SQLite (kann leicht angepasst werden)
- **Tests:** xUnit, Integrationstests
- **DevOps:** Docker, Docker Compose, .gitignore optimiert

---

## Schnellstart

```bash
git clone https://github.com/BTBlueTiger/Mini-Warehouse-Dashboard.git
cd Mini-Warehouse-Dashboard
docker compose up --build
```

- Die Anwendung ist dann unter `http://localhost:5000` (API, nicht unter Docker) und `http://localhost:8080` (Client) erreichbar.
- Standard-Admin:  
	- **E-Mail:** admin@MiniWarehouse.com  
	- **Passwort:** admin@MiniWarehouse.com

---

## Was du noch wissen solltest

- JWT wird als httpOnly-Cookie gesetzt (sicher gegen XSS).
- Die Navigation passt sich automatisch der Rolle an.
- Das System ist erweiterbar für weitere Rollen und Features.
- **Schwächen:** Hardcodierter JWT-Key, keine 2FA, keine E-Mail-Bestätigung, keine produktive Security-Konfiguration.

---

**Viel Spaß beim Ausprobieren!**
# Mini-Warehouse-Dashboard
