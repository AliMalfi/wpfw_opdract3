using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using wpfw_opdracht3.Auth;
using wpfw_opdracht3.DTOs;
using wpfw_opdracht3.Models;

namespace wpfw_opdracht3.Controllers;

[Route("api/[controller]")]   // adres: api/auth
[ApiController]
public class AuthController : ControllerBase
{
    // UserManager = hulp van Identity om gebruikers te maken en beheren
    private readonly UserManager<User> _userManager;

    // SignInManager = hulp van Identity om in/uit te loggen en de cookie te maken
    private readonly SignInManager<User> _signInManager;

    // Beide komen binnen via Dependency Injection
    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // POST: api/auth/register -> nieuwe gebruiker met rol Gast
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        // 1. Nieuwe gebruiker maken (e-mail is ook de gebruikersnaam)
        var user = new User
        {
            UserName = dto.Email,
            Email = dto.Email
        };

        // 2. Opslaan in de database. Het wachtwoord wordt automatisch gehasht
        var result = await _userManager.CreateAsync(user, dto.Password);

        // 3. Mislukt? (bijv. te zwak wachtwoord of e-mail bestaat al) -> 400
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        // 4. Iedere nieuwe gebruiker krijgt de rol Gast (nooit Editor of Admin)
        await _userManager.AddToRoleAsync(user, Roles.Gast);

        // 5. Gelukt -> 200
        return Ok(new { message = "Registratie gelukt", email = user.Email, rol = Roles.Gast });
    }

    // POST: api/auth/login -> wachtwoord controleren en cookie geven
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        // 1. Controleer e-mail + wachtwoord. Klopt het? Dan maakt Identity de cookie
        var result = await _signInManager.PasswordSignInAsync(
            dto.Email,               // gebruikersnaam (= e-mail)
            dto.Password,            // wachtwoord
            isPersistent: false,     // cookie verdwijnt als de browser sluit
            lockoutOnFailure: false  // niet blokkeren na foute pogingen
        );

        // 2. Klopt niet? -> 401 (we zeggen niet wát er fout is, voor de veiligheid)
        if (!result.Succeeded)
        {
            return Unauthorized(new { message = "E-mail of wachtwoord is onjuist" });
        }

        // 3. Gelukt -> 200 + de browser krijgt de cookie
        return Ok(new { message = "Inloggen gelukt" });
    }

    // POST: api/auth/logout -> cookie weghalen
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // 1. Cookie weghalen (uitloggen)
        await _signInManager.SignOutAsync();

        // 2. Gelukt -> 200
        return Ok(new { message = "Uitgelogd" });
    }
}