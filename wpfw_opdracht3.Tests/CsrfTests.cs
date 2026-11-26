using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace wpfw_opdracht3.Tests;

[Collection("Api")]
public class CsrfTests
{
    private readonly ApiFactory _factory;

    public CsrfTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CsrfEndpoint_GeeftToken()
    {
        var client = _factory.MaakClientZonderCsrf();

        var response = await client.GetAsync("/api/security/csrf");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.False(string.IsNullOrEmpty(json.GetProperty("token").GetString()));
    }

    [Fact]
    public async Task Post_ZonderCsrfToken_Geeft400()
    {
        var client = _factory.MaakClientZonderCsrf();

        var response = await client.PostAsJsonAsync("/api/Auth/login",
            new { email = "iemand@test.nl", password = "Fout123!" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}