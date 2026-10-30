using System.Net;
using System.Net.Http.Json;
using wpfw_opdracht3.Auth;
using wpfw_opdracht3.DTOs;

namespace wpfw_opdracht3.Tests;

// Tests voor /api/projects. [Collection("Api")] = gebruik de gedeelde ApiFactory
[Collection("Api")]
public class ProjectsTests
{
    private readonly ApiFactory _factory;

    public ProjectsTests(ApiFactory factory)
    {
        _factory = factory;
    }

    // ---------- Hulpmethodes ----------

    // Maakt een gebruiker met deze rol, logt in en geeft een ingelogde nep-browser terug
    private async Task<HttpClient> IngelogdeClientAsync(string email, string rol)
    {
        await _factory.MaakGebruikerAsync(email, rol);

        var client = _factory.MaakClient();
        var login = new LoginDto { Email = email, Password = ApiFactory.TestWachtwoord };
        var response = await client.PostAsJsonAsync("/api/auth/login", login);
        response.EnsureSuccessStatusCode();   // stop als inloggen mislukt

        return client;   // deze client heeft nu de cookie
    }

    // Een geldig project om mee te testen
    private static ProjectCreateDto GeldigProject(string titel) => new()
    {
        Titel = titel,
        Beschrijving = "Testbeschrijving",
        Categorie = "Test",
        GithubUrl = "",
        Datum = new DateTime(2026, 10, 30)
    };

    // ---------- GET (publiek) ----------

    [Fact]
    public async Task GetAll_ZonderInloggen_Geeft200()
    {
        // Arrange
        var client = _factory.MaakClient();

        // Act
        var response = await client.GetAsync("/api/projects");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ---------- Autorisatie bij POST ----------

    [Fact]
    public async Task Create_ZonderInloggen_Geeft401()
    {
        // Arrange: niet ingelogd
        var client = _factory.MaakClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", GeldigProject("Mag niet"));

        // Assert: "wie ben jij?"
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_AlsGast_Geeft403()
    {
        // Arrange: ingelogd als Gast
        var client = await IngelogdeClientAsync("gast-p@test.nl", Roles.Gast);

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", GeldigProject("Mag niet"));

        // Assert: "jij mag dit niet"
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Create_AlsEditor_Geeft201EnSlaatOp()
    {
        // Arrange: ingelogd als Editor
        var client = await IngelogdeClientAsync("editor-p@test.nl", Roles.Editor);

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", GeldigProject("Door Editor"));

        // Assert 1: 201 Created
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Assert 2: het teruggestuurde project heeft een Id en de juiste titel
        var gemaakt = await response.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.NotNull(gemaakt);
        Assert.True(gemaakt.Id > 0);
        Assert.Equal("Door Editor", gemaakt.Titel);

        // Assert 3: het project staat echt in de database (GET geeft hem terug)
        var opgehaald = await client.GetFromJsonAsync<ProjectDto>($"/api/projects/{gemaakt.Id}");
        Assert.Equal("Door Editor", opgehaald!.Titel);
    }

    [Fact]
    public async Task Create_AlsAdmin_Geeft201()
    {
        // Arrange: ingelogd als Admin
        var client = await IngelogdeClientAsync("admin-p@test.nl", Roles.Admin);

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", GeldigProject("Door Admin"));

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}