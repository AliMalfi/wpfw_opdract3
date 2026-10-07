namespace wpfw_opdracht3.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Titel { get; set; } = "";
        public string Beschrijving { get; set; } = "";
        public string Categorie { get; set; } = "";
        public string GithubUrl { get; set; } = "";
        public DateTime Datum { get; set; }
    }
}