using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using wpfw_opdracht3.Auth;
using wpfw_opdracht3.DTOs;
using wpfw_opdracht3.Models;

namespace wpfw_opdracht3.Tests;

// Tests voor /api/auth (register, login, logout)
[Collection("Api")]
public class AuthTests
{
    private readonly ApiFactory _factory;

    public AuthTests(ApiFactory factory)
    {
        _factory = factory;
    }

    // ---------- REGISTER ----------

    [Fact]
    public async Task Register_NieuweGebruiker_Geeft200EnRolGast()
    {
        // Arrange
        var client = _factory.MaakClient();
        var dto = new RegisterDto { Email = "nieuw@test.nl", Password = ApiFactory.TestWachtwoord };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", dto);

        // Assert 1: 200 OK
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Assert 2: in de database heeft de gebruiker de rol Gast, en NIET Editor of Admin
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = await userManager.FindByEmailAsync("nieuw@test.nl");
        Assert.NotNull(user);
        Assert.True(await userManager.IsInRoleAsync(user, Roles.Gast));
        Assert.False(await userManager.IsInRoleAsync(user, Roles.Editor));
        Assert.False(await userManager.IsInRoleAsync(user, Roles.Admin));
    }

    [Fact]
    public async Task Register_BestaandEmail_Geeft400()
    {
        // Arrange: eerst één keer registreren
        var client = _factory.MaakClient();
        var dto = new RegisterDto { Email = "dubbel@test.nl", Password = ApiFactory.TestWachtwoord };
        await client.PostAsJsonAsync("/api/auth/register", dto);

        // Act: nog een keer met hetzelfde e-mailadres
        var response = await client.PostAsJsonAsync("/api/auth/register", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_ZwakWachtwoord_Geeft400()
    {
        // Arrange: wachtwoord zonder hoofdletter, cijfer en speciaal teken
        var client = _factory.MaakClient();
        var dto = new RegisterDto { Email = "zwak@test.nl", Password = "zwak" };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ---------- LOGIN ----------

    [Fact]
    public async Task Login_JuistWachtwoord_Geeft200EnCookie()
    {
        // Arrange: gebruiker bestaat, client die cookies NIET zelf verwerkt (zodat we de header kunnen zien)
        await _factory.MaakGebruikerAsync("login@test.nl", Roles.Gast);
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            HandleCookies = false
        });
        var dto = new LoginDto { Email = "login@test.nl", Password = ApiFactory.TestWachtwoord };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", dto);

        // Assert 1: 200 OK
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Assert 2: de API stuurt de authenticatiecookie mee
        Assert.True(response.Headers.TryGetValues("Set-Cookie", out var cookies));
        Assert.Contains(cookies!, c => c.StartsWith(".AspNetCore.Identity.Application"));
    }

    [Fact]
    public async Task Login_FoutWachtwoord_Geeft401()
    {
        // Arrange
        await _factory.MaakGebruikerAsync("fout@test.nl", Roles.Gast);
        var client = _factory.MaakClient();
        var dto = new LoginDto { Email = "fout@test.nl", Password = "Verkeerd123!" };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ---------- LOGOUT ----------

    [Fact]
    public async Task Logout_DaarnaSchrijven_Geeft401()
    {
        // Arrange: inloggen als Editor en daarna uitloggen
        await _factory.MaakGebruikerAsync("uitlog@test.nl", Roles.Editor);
        var client = _factory.MaakClient();
        var login = new LoginDto { Email = "uitlog@test.nl", Password = ApiFactory.TestWachtwoord };
        await client.PostAsJsonAsync("/api/auth/login", login);
        await client.PostAsync("/api/auth/logout", null);

        // Act: proberen een blogpost aan te maken
        var blogpost = new BlogpostCreateDto { Titel = "Na logout", Inhoud = "Mag niet", Publicatiedatum = DateTime.Today };
        var response = await client.PostAsJsonAsync("/api/blogposts", blogpost);

        // Assert: cookie is weg -> 401
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}