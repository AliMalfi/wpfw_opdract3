using System.ComponentModel.DataAnnotations;

namespace wpfw_opdracht3.DTOs
{
    public class ProjectCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string Titel { get; set; } = "";

        [Required]
        public string Beschrijving { get; set; } = "";

        [Required]
        public string Categorie { get; set; } = "";

        public string GithubUrl { get; set; } = "";

        public DateTime Datum { get; set; }
    }
}