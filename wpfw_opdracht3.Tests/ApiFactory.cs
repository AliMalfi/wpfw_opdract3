using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using wpfw_opdracht3.Data;
using wpfw_opdracht3.Models;

namespace wpfw_opdracht3.Tests;

// Start de hele API op voor de tests, maar met een APARTE testdatabase
public class ApiFactory : WebApplicationFactory<Program>
{
    // Testdatabase: PortfolioDb_Test (NIET je echte PortfolioDb)
    private const string TestConnectionString =
        "Server=.\\SQLEXPRESS;Database=PortfolioDb_Test;Trusted_Connection=True;TrustServerCertificate=True;";

    // Wachtwoord voor de testgebruikers (alleen voor de tijdelijke testdatabase)
    public const string TestWachtwoord = "Test123!";

    public ApiFactory()
    {
        // 1. Oude testdatabase weggooien, zodat elke testrun schoon begint
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(TestConnectionString)
            .Options;
        using var db = new AppDbContext(options);
        db.Database.EnsureDeleted();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // 2. De API laten werken met de testdatabase in plaats van de echte
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = TestConnectionString
            });
        });
    }

    // 3. Een "nep-browser" maken die cookies onthoudt (zoals Chrome)
    public HttpClient MaakClient()
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            HandleCookies = true
        });
    }

    // 4. Testgebruiker met een bepaalde rol aanmaken (als hij nog niet bestaat)
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

// Alle testklassen delen één fabriek en draaien NA elkaar (niet tegelijk),
// zodat ze niet tegelijk in dezelfde testdatabase werken
[CollectionDefinition("Api")]
public class ApiCollection : ICollectionFixture<ApiFactory>
{
}