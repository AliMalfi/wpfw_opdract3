using System.ComponentModel.DataAnnotations;

namespace wpfw_opdracht3.DTOs
{
    public class BlogpostCreateDto
    {
        //betekent dit veld mag niet leeg zijn
        [Required]
        [MaxLength(200)]
        public string Titel { get; set; } = "";

        
        [Required]
        public string Inhoud { get; set; } = "";

        public DateTime Publicatiedatum { get; set; }

    }
}
