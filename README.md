# wpfw_opdract3 – Portfolio Backend (beveiligd en getest)

ASP.NET Core Web API voor mijn portfoliosite. De API slaat projecten en blogposts op in SQL Server via Entity Framework Core, gebruikt **ASP.NET Core Identity** voor accounts en rollen, en is getest met een **xUnit**-testproject.

**Student:** Ali Malfi

**Portfolio (frontend):** https://github.com/AliMalfi/AliMalfi.github.io

## Technieken
- .NET 10 – ASP.NET Core Web API (controllers)
- Entity Framework Core 10 + SQL Server Express
- ASP.NET Core Identity (gebruikers, rollen, cookie-login)
- Swagger UI (Swashbuckle.AspNetCore)
- xUnit + Microsoft.AspNetCore.Mvc.Testing (integratietests via HTTP)

## Projectstructuur
| Map / project | Inhoud |
|---|---|
| `wpfw_opdract3/Controllers` | `ProjectsController`, `BlogpostsController`, `AuthController` |
| `wpfw_opdract3/Services` | Logica + omzetting entiteit ↔ DTO |
| `wpfw_opdract3/Data` | `AppDbContext` (IdentityDbContext) en `RoleSeeder` |
| `wpfw_opdract3/Models` | `Project`, `Blogpost`, `User` |
| `wpfw_opdract3/DTOs` | DTO's voor projecten, blogposts, register en login |
| `wpfw_opdract3/Auth` | Rolnamen (`Roles`) en policy-namen (`Policies`) |
| `wpfw_opdract3/Scripts` | `rollen-toekennen.sql` (lokaal Editor/Admin maken) |
| `wpfw_opdracht3.Tests` | xUnit-testproject (33 tests) |

## Vereisten
- .NET 10 SDK
- SQL Server Express (`.\SQLEXPRESS`), inloggen met Windows-account
- SQL Server Management Studio (voor het rolscript)

## 1. Database en migraties
De connectionstring staat in `appsettings.json`:

```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=PortfolioDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

De database en alle tabellen (ook de **Identity-tabellen** zoals `AspNetUsers` en `AspNetRoles`) worden **automatisch aangemaakt bij het opstarten**: `RoleSeeder` voert `Database.MigrateAsync()` uit en maakt daarna de rollen **Gast**, **Editor** en **Admin** aan.

Handmatig migreren kan ook:

```bash
dotnet ef database update --project wpfw_opdract3
```

of in Visual Studio (Package Manager Console): `Update-Database`

## 2. API starten
```bash
dotnet run --project wpfw_opdract3
```
Swagger UI opent op de startpagina: `https://localhost:7101/`

## 3. Gebruikers en rollen lokaal beschikbaar maken
Er staan **geen wachtwoorden of sleutels** in de repository.

1. **Gewone gebruiker (Gast):** registreer via `POST /api/auth/register`.
   Iedere nieuwe gebruiker krijgt automatisch de rol **Gast**. Publieke registratie kent nooit Editor of Admin toe.
2. **Editor en Admin:** registreer eerst `editor@test.nl` en `admin@test.nl` via `POST /api/auth/register` (met een eigen wachtwoord).
3. Open `wpfw_opdract3/Scripts/rollen-toekennen.sql` in SQL Server Management Studio en voer het uit (**F5**). Het script vervangt de rol Gast door **Editor** of **Admin** en toont daarna welke gebruiker welke rol heeft.
4. **Log opnieuw in** (`POST /api/auth/logout`, daarna `POST /api/auth/login`). De rollen worden bij het inloggen in de cookie gezet, dus na een rolwijziging is een nieuwe login nodig.

Wachtwoordregels (Identity): minimaal 6 tekens, met hoofdletter, kleine letter, cijfer en speciaal teken.

## 4. Inloggen en autorisatie
- `POST /api/auth/login` geeft een **authenticatiecookie** (`.AspNetCore.Identity.Application`). Swagger en de browser sturen die automatisch mee.
- **GET**-routes zijn publiek.
- **POST, PUT en DELETE** vereisen de policy `CanEdit` (rol **Editor** of **Admin**).
  - Zonder aanmelding → **401 Unauthorized**
  - Aangemeld zonder Editor/Admin → **403 Forbidden**

## 5. Tests uitvoeren
Vanuit de solutionmap:
```bash
dotnet test
```
Of in Visual Studio: **Test → Test Explorer → Run All Tests**.

- De tests starten de API met `WebApplicationFactory` en sturen echte HTTP-requests.
- Ze gebruiken een **aparte testdatabase** `PortfolioDb_Test`, die bij elke testrun wordt verwijderd en opnieuw aangemaakt. De gewone database `PortfolioDb` wordt niet aangeraakt.
- Het testwachtwoord in `ApiFactory.cs` hoort alleen bij deze tijdelijke testdatabase.

## Endpoints
| Methode | Route | Toegang |
|---|---|---|
| GET | `/api/projects` | iedereen |
| GET | `/api/projects/{id}` | iedereen |
| POST | `/api/projects` | Editor, Admin |
| PUT | `/api/projects/{id}` | Editor, Admin |
| DELETE | `/api/projects/{id}` | Editor, Admin |
| GET | `/api/blogposts` | iedereen |
| GET | `/api/blogposts/{id}` | iedereen |
| POST | `/api/blogposts` | Editor, Admin |
| PUT | `/api/blogposts/{id}` | Editor, Admin |
| DELETE | `/api/blogposts/{id}` | Editor, Admin |
| POST | `/api/auth/register` | iedereen (krijgt rol Gast) |
| POST | `/api/auth/login` | iedereen |
| POST | `/api/auth/logout` | iedereen |