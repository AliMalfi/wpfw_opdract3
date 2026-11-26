using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.Testing.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using wpfw_opdracht3.Data;
using wpfw_opdracht3.Models;

namespace wpfw_opdracht3.Tests;

// Start de hele API op voor de tests, maar met een APARTE testdatabase
public class ApiFactory : WebApplicationFactory<Program>
{
    // 1. Testdatabase: lokaal SQLEXPRESS, in GitHub Actions via TEST_CONNECTION_STRING
    private static readonly string TestConnectionString =
        Environment.GetEnvironmentVariable("TEST_CONNECTION_STRING")
        ?? "Server=.\\SQLEXPRESS;Database=PortfolioDb_Test;Trusted_Connection=True;TrustServerCertificate=True;";

    // Wachtwoord voor de testgebruikers (alleen voor de tijdelijke testdatabase)
    public const string TestWachtwoord = "Test123!";

    public ApiFactory()
    {
        // 2. Oude testdatabase weggooien, zodat elke testrun schoon begint
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(TestConnectionString)
            .Options;
        using var db = new AppDbContext(options);
        db.Database.EnsureDeleted();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // 3. De API laten werken met de testdatabase in plaats van de echte
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = TestConnectionString
            });
        });
    }

    // 4. "Nep-browser" die cookies onthoudt EN automatisch het CSRF-token meestuurt
    public HttpClient MaakClient()
    {
        return CreateDefaultClient(
            new Uri("https://localhost"),
            new CsrfHandler(),            // eerst: token ophalen en in de header zetten
            new CookieContainerHandler()  // daarna: cookies onthouden (zoals Chrome)
        );
    }

    // 5. Nep-browser ZONDER CSRF-token (om te testen dat de beveiliging werkt)
    public HttpClient MaakClientZonderCsrf()
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            HandleCookies = true
        });
    }

    // 6. Testgebruiker met een bepaalde rol aanmaken (als hij nog niet bestaat)
    public async Task MaakGebruikerAsync(string email, string rol)
    {
        using var scope = Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        if (await userManager.FindByEmailAsync(email) is null)
        {
            var user = new User { UserName = email, Email = email };
            await userManager.CreateAsync(user, TestWachtwoord);
            await userManager.AddToRoleAsync(user, rol);
        }
    }
}

// 7. Doet hetzelfde als client.js in React: vóór POST/PUT/DELETE eerst een token ophalen
public class CsrfHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Method != HttpMethod.Get && request.Method != HttpMethod.Head)
        {
            var tokenRequest = new HttpRequestMessage(
                HttpMethod.Get, new Uri(request.RequestUri!, "/api/security/csrf"));
            var tokenResponse = await base.SendAsync(tokenRequest, cancellationToken);
            var json = await tokenResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);

            request.Headers.Add("X-CSRF-TOKEN", json.GetProperty("token").GetString());
        }

        return await base.SendAsync(request, cancellationToken);
    }
}

// Alle testklassen delen één fabriek en draaien NA elkaar (niet tegelijk),
// zodat ze niet tegelijk in dezelfde testdatabase werken
[CollectionDefinition("Api")]
public class ApiCollection : ICollectionFixture<ApiFactory>
{
}