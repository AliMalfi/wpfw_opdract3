using wpfw_opdracht3.Data;
using wpfw_opdracht3.DTOs;
using wpfw_opdracht3.Models;

namespace wpfw_opdracht3.Services;

// Service = de "kok": bevat de logica voor blogposts
public class BlogpostService : IBlogpostService
{
    // Verbinding met de database
    private readonly AppDbContext _context;

    // DbContext komt binnen via Dependency Injection
    public BlogpostService(AppDbContext context)
    {
        _context = context;
    }

    // Alle blogposts ophalen als DTO's
    public List<BlogpostDto> GetAll()
    {
        return _context.Blogposts
            .Select(b => ToDto(b))   // elke blogpost -> DTO
            .ToList();               // in een lijst stoppen
    }

    // Eén blogpost ophalen, of null als hij niet bestaat
    public BlogpostDto? GetById(int id)
    {
        var blogpost = _context.Blogposts.Find(id);           // zoek op id
        return blogpost is null ? null : ToDto(blogpost);     // niet gevonden? null
    }

    // Nieuwe blogpost opslaan
    public BlogpostDto Create(BlogpostCreateDto dto)
    {
        var blogpost = new Blogpost           // nieuwe entiteit maken
        {
            Titel = dto.Titel,                // waarden uit DTO overnemen
            Inhoud = dto.Inhoud,
            Publicatiedatum = dto.Publicatiedatum
        };                                    // Id maakt de database zelf

        _context.Blogposts.Add(blogpost);     // klaarzetten
        _context.SaveChanges();               // echt opslaan, Id wordt gemaakt

        return ToDto(blogpost);               // teruggeven mét Id
    }

    // Bestaande blogpost wijzigen. true = gelukt, false = niet gevonden
    public bool Update(int id, BlogpostCreateDto dto)
    {
        var blogpost = _context.Blogposts.Find(id);   // zoek op id
        if (blogpost is null) return false;           // niet gevonden

        blogpost.Titel = dto.Titel;                   // nieuwe waarden zetten
        blogpost.Inhoud = dto.Inhoud;
        blogpost.Publicatiedatum = dto.Publicatiedatum;

        _context.SaveChanges();                       // wijziging opslaan
        return true;
    }

    // Blogpost verwijderen. true = gelukt, false = niet gevonden
    public bool Delete(int id)
    {
        var blogpost = _context.Blogposts.Find(id);   // zoek op id
        if (blogpost is null) return false;           // niet gevonden

        _context.Blogposts.Remove(blogpost);          
        _context.SaveChanges();                       
        return true;
    }

    // Zet entiteit om naar DTO (op één plek)
    private static BlogpostDto ToDto(Blogpost b)
    {
        return new BlogpostDto
        {
            Id = b.Id,
            Titel = b.Titel,
            Inhoud = b.Inhoud,
            Publicatiedatum = b.Publicatiedatum
        };
    }
}