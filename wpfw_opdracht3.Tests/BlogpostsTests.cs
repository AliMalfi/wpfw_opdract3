using System.Net;
using System.Net.Http.Json;
using wpfw_opdracht3.Auth;
using wpfw_opdracht3.DTOs;

namespace wpfw_opdracht3.Tests;

// Tests voor /api/blogposts. [Collection("Api")] = gebruik de gedeelde ApiFactory
[Collection("Api")]
public class BlogpostsTests
{
    private readonly ApiFactory _factory;

    public BlogpostsTests(ApiFactory factory)
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
        response.EnsureSuccessStatusCode();

        return client;
    }

    // Een geldige blogpost om mee te testen
    private static BlogpostCreateDto GeldigeBlogpost(string titel) => new()
    {
        Titel = titel,
        Inhoud = "Testinhoud",
        Publicatiedatum = new DateTime(2026, 10, 30)
    };

    // Maakt (als Editor) een blogpost aan en geeft hem terug
    private async Task<BlogpostDto> MaakBlogpostAsync(string titel)
    {
        var editor = await IngelogdeClientAsync("editor-b@test.nl", Roles.Editor);
        var response = await editor.PostAsJsonAsync("/api/blogposts", GeldigeBlogpost(titel));
        return (await response.Content.ReadFromJsonAsync<BlogpostDto>())!;
    }

    // ---------- GET (publiek) ----------

    [Fact]
    public async Task GetAll_ZonderInloggen_Geeft200MetSeedData()
    {
        // Arrange
        var client = _factory.MaakClient();

        // Act
        var response = await client.GetAsync("/api/blogposts");

        // Assert 1: 200 OK
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Assert 2: de 2 seed-blogposts staan erin
        var lijst = await response.Content.ReadFromJsonAsync<List<BlogpostDto>>();
        Assert.Contains(lijst!, b => b.Titel == "Mijn eerste stappen met React");
    }

    [Fact]
    public async Task GetById_BestaandId_GeeftJuisteBlogpost()
    {
        // Arrange
        var client = _factory.MaakClient();

        // Act: blogpost 1 komt uit de seed-data
        var blogpost = await client.GetFromJsonAsync<BlogpostDto>("/api/blogposts/1");

        // Assert: de juiste gegevens
        Assert.NotNull(blogpost);
        Assert.Equal(1, blogpost.Id);
        Assert.Equal("Mijn eerste stappen met React", blogpost.Titel);
    }

    [Fact]
    public async Task GetById_OnbekendId_Geeft404()
    {
        // Arrange
        var client = _factory.MaakClient();

        // Act
        var response = await client.GetAsync("/api/blogposts/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ---------- POST ----------

    [Fact]
    public async Task Create_ZonderInloggen_Geeft401()
    {
        // Arrange
        var client = _factory.MaakClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", GeldigeBlogpost("Mag niet"));

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_AlsGast_Geeft403()
    {
        // Arrange
        var client = await IngelogdeClientAsync("gast-b@test.nl", Roles.Gast);

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", GeldigeBlogpost("Mag niet"));

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Create_AlsEditor_Geeft201EnSlaatOp()
    {
        // Arrange
        var client = await IngelogdeClientAsync("editor-b@test.nl", Roles.Editor);

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", GeldigeBlogpost("Blog door Editor"));

        // Assert 1: 201 Created
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        // Assert 2: de teruggestuurde blogpost heeft een Id en de juiste titel
        var gemaakt = await response.Content.ReadFromJsonAsync<BlogpostDto>();
        Assert.NotNull(gemaakt);
        Assert.True(gemaakt.Id > 0);
        Assert.Equal("Blog door Editor", gemaakt.Titel);

        // Assert 3: hij staat echt in de database
        var opgehaald = await client.GetFromJsonAsync<BlogpostDto>($"/api/blogposts/{gemaakt.Id}");
        Assert.Equal("Blog door Editor", opgehaald!.Titel);
    }

    [Fact]
    public async Task Create_AlsAdmin_Geeft201()
    {
        // Arrange
        var client = await IngelogdeClientAsync("admin-b@test.nl", Roles.Admin);

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", GeldigeBlogpost("Blog door Admin"));

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Create_LegeInhoud_Geeft400()
    {
        // Arrange: inhoud is leeg
        var client = await IngelogdeClientAsync("editor-b@test.nl", Roles.Editor);
        var blogpost = GeldigeBlogpost("Titel");
        blogpost.Inhoud = "";

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", blogpost);

        // Assert: [Required] op Inhoud -> 400
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ---------- PUT ----------

    [Fact]
    public async Task Update_AlsGast_Geeft403()
    {
        // Arrange
        var bestaand = await MaakBlogpostAsync("Origineel");
        var client = await IngelogdeClientAsync("gast-b@test.nl", Roles.Gast);

        // Act
        var response = await client.PutAsJsonAsync($"/api/blogposts/{bestaand.Id}", GeldigeBlogpost("Gewijzigd"));

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Update_AlsAdmin_Geeft204EnWijzigtGegevens()
    {
        // Arrange
        var bestaand = await MaakBlogpostAsync("Voor wijziging");
        var client = await IngelogdeClientAsync("admin-b@test.nl", Roles.Admin);

        // Act
        var response = await client.PutAsJsonAsync($"/api/blogposts/{bestaand.Id}", GeldigeBlogpost("Na wijziging"));

        // Assert 1: 204 No Content
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Assert 2: de titel is echt veranderd
        var opgehaald = await client.GetFromJsonAsync<BlogpostDto>($"/api/blogposts/{bestaand.Id}");
        Assert.Equal("Na wijziging", opgehaald!.Titel);
    }

    [Fact]
    public async Task Update_OnbekendId_Geeft404()
    {
        // Arrange
        var client = await IngelogdeClientAsync("editor-b@test.nl", Roles.Editor);

        // Act
        var response = await client.PutAsJsonAsync("/api/blogposts/999999", GeldigeBlogpost("Bestaat niet"));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ---------- DELETE ----------

    [Fact]
    public async Task Delete_ZonderInloggen_Geeft401()
    {
        // Arrange
        var bestaand = await MaakBlogpostAsync("Mag niet weg");
        var client = _factory.MaakClient();

        // Act
        var response = await client.DeleteAsync($"/api/blogposts/{bestaand.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Delete_AlsEditor_Geeft204EnVerwijdert()
    {
        // Arrange
        var bestaand = await MaakBlogpostAsync("Gaat weg");
        var client = await IngelogdeClientAsync("editor-b@test.nl", Roles.Editor);

        // Act
        var response = await client.DeleteAsync($"/api/blogposts/{bestaand.Id}");

        // Assert 1: 204 No Content
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Assert 2: hij is echt weg (GET geeft 404)
        var check = await client.GetAsync($"/api/blogposts/{bestaand.Id}");
        Assert.Equal(HttpStatusCode.NotFound, check.StatusCode);
    }

    [Fact]
    public async Task Delete_OnbekendId_Geeft404()
    {
        // Arrange
        var client = await IngelogdeClientAsync("admin-b@test.nl", Roles.Admin);

        // Act
        var response = await client.DeleteAsync("/api/blogposts/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}