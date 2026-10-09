# wpfw_opdract3 – Portfolio Backend

ASP.NET Core Web API voor mijn portfoliosite. De API slaat projecten en blogposts op in een SQL Server-database via Entity Framework Core.

**Student:** Ali Malfi
**Portfolio (frontend):** https://github.com/AliMalfi/AliMalfi.github.io

## Technieken
- .NET 10 – ASP.NET Core Web API (controllers)
- Entity Framework Core 10 met SQL Server (SQL Express)
- Swagger UI (Swashbuckle.AspNetCore)

## Opbouw (lagen)
| Map | Rol |
|---|---|
| `Controllers` | Ontvangt HTTP-requests en geeft HTTP-responses terug |
| `Services` | Logica + omzetting tussen entiteit en DTO |
| `Data` | `AppDbContext` – verbinding met de database via EF Core |
| `Models` | Entiteiten (`Project`, `Blogpost`) |
| `DTOs` | `ProjectDto`, `ProjectCreateDto`, `BlogpostDto` |

## Vereisten
- .NET 10 SDK
- SQL Server Express (of LocalDB)
- EF Core tools: `dotnet tool install --global dotnet-ef`

## Database aanmaken
De database wordt aangemaakt met **migraties**. Controleer eerst de connectionstring in `appsettings.json`:

```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=PortfolioDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

Voer daarna in de projectmap (`wpfw_opdract3/wpfw_opdracht3`) uit:

```bash
dotnet ef database update
```

Of in Visual Studio via de Package Manager Console: `Update-Database`

Hiermee worden de tabellen `Projects` en `Blogposts` aangemaakt en twee voorbeeld-blogposts toegevoegd.

## Applicatie starten
```bash
dotnet run
```
Open daarna Swagger UI op de URL die in de console verschijnt, gevolgd door `/swagger`
(bijvoorbeeld `http://localhost:5058/swagger`).

## Endpoints
| Methode | Route | Omschrijving |
|---|---|---|
| GET | `/api/projects` | Alle projecten |
| GET | `/api/projects/{id}` | Eén project |
| POST | `/api/projects` | Nieuw project aanmaken |
| PUT | `/api/projects/{id}` | Project wijzigen |
| DELETE | `/api/projects/{id}` | Project verwijderen |
| GET | `/api/blogposts` | Alle blogposts |
| GET | `/api/blogposts/{id}` | Eén blogpost |