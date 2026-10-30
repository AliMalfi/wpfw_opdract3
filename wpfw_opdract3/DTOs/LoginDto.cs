using System.ComponentModel.DataAnnotations;

namespace wpfw_opdracht3.DTOs;

// Formulier om in te loggen
public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}