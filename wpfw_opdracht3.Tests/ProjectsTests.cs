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

    // Maakt (als Editor) een project aan en geeft het terug, zodat we het kunnen wijzigen/verwijderen
    private async Task<ProjectDto> MaakProjectAsync(string titel)
    {
        var editor = await IngelogdeClientAsync("editor-p@test.nl", Roles.Editor);
        var response = await editor.PostAsJsonAsync("/api/projects", GeldigProject(titel));
        return (await response.Content.ReadFromJsonAsync<ProjectDto>())!;
    }

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

    [Fact]
    public async Task GetById_OnbekendId_Geeft404()
    {
        // Arrange
        var client = _factory.MaakClient();

        // Act: een id dat niet bestaat
        var response = await client.GetAsync("/api/projects/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ---------- POST ----------

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

    [Fact]
    public async Task Create_LegeTitel_Geeft400()
    {
        // Arrange: ingelogd als Editor, maar titel is leeg
        var client = await IngelogdeClientAsync("editor-p@test.nl", Roles.Editor);
        var project = GeldigProject("");

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", project);

        // Assert: [Required] op Titel -> 400 Bad Request
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ---------- PUT ----------

    [Fact]
    public async Task Update_ZonderInloggen_Geeft401()
    {
        // Arrange
        var bestaand = await MaakProjectAsync("Origineel");
        var client = _factory.MaakClient();   // niet ingelogd

        // Act
        var response = await client.PutAsJsonAsync($"/api/projects/{bestaand.Id}", GeldigProject("Gewijzigd"));

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Update_AlsEditor_Geeft204EnWijzigtGegevens()
    {
        // Arrange
        var bestaand = await MaakProjectAsync("Voor wijziging");
        var client = await IngelogdeClientAsync("editor-p@test.nl", Roles.Editor);

        // Act
        var response = await client.PutAsJsonAsync($"/api/projects/{bestaand.Id}", GeldigProject("Na wijziging"));

        // Assert 1: 204 No Content
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Assert 2: de titel is echt veranderd in de database
        var opgehaald = await client.GetFromJsonAsync<ProjectDto>($"/api/projects/{bestaand.Id}");
        Assert.Equal("Na wijziging", opgehaald!.Titel);
    }

    [Fact]
    public async Task Update_OnbekendId_Geeft404()
    {
        // Arrange
        var client = await IngelogdeClientAsync("editor-p@test.nl", Roles.Editor);

        // Act
        var response = await client.PutAsJsonAsync("/api/projects/999999", GeldigProject("Bestaat niet"));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ---------- DELETE ----------

    [Fact]
    public async Task Delete_AlsGast_Geeft403()
    {
        // Arrange
        var bestaand = await MaakProjectAsync("Mag niet weg");
        var client = await IngelogdeClientAsync("gast-p@test.nl", Roles.Gast);

        // Act
        var response = await client.DeleteAsync($"/api/projects/{bestaand.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Delete_AlsAdmin_Geeft204EnVerwijdert()
    {
        // Arrange
        var bestaand = await MaakProjectAsync("Gaat weg");
        var client = await IngelogdeClientAsync("admin-p@test.nl", Roles.Admin);

        // Act
        var response = await client.DeleteAsync($"/api/projects/{bestaand.Id}");

        // Assert 1: 204 No Content
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Assert 2: het project is echt weg (GET geeft 404)
        var check = await client.GetAsync($"/api/projects/{bestaand.Id}");
        Assert.Equal(HttpStatusCode.NotFound, check.StatusCode);
    }

    [Fact]
    public async Task Delete_OnbekendId_Geeft404()
    {
        // Arrange
        var client = await IngelogdeClientAsync("admin-p@test.nl", Roles.Admin);

        // Act
        var response = await client.DeleteAsync("/api/projects/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}