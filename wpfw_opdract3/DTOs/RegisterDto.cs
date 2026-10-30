using System.ComponentModel.DataAnnotations;

namespace wpfw_opdracht3.DTOs;

// Formulier om een nieuw account te maken
public class RegisterDto
{
    [Required]
    [EmailAddress]          // moet een geldig e-mailadres zijn (met @)
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}