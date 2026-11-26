using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace wpfw_opdracht3.Controllers;

[ApiController]
[Route("api/security")]
public class SecurityController(IAntiforgery antiforgery) : ControllerBase
{
    // GET /api/security/csrf
    // 1. Zet een CSRF-cookie (HttpOnly) en
    // 2. geeft het bijpassende token terug, dat React in de header X-CSRF-TOKEN zet
    [HttpGet("csrf")]
    public IActionResult GetCsrfToken()
    {
        var tokens = antiforgery.GetAndStoreTokens(HttpContext);
        return Ok(new { token = tokens.RequestToken });
    }
}