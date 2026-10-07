using wpfw_opdracht3.Data;
using wpfw_opdracht3.DTOs;
using wpfw_opdracht3.Models;

namespace wpfw_opdracht3.Services;

public class BlogpostService : IBlogpostService
{
    private readonly AppDbContext _context;

    public BlogpostService(AppDbContext context)
    {
        _context = context;
    }

    public List<BlogpostDto> GetAll()
    {
        return _context.Blogposts
            .Select(b => ToDto(b))
            .ToList();
    }

    public BlogpostDto? GetById(int id)
    {
        var blogpost = _context.Blogposts.Find(id);
        return blogpost is null ? null : ToDto(blogpost);
    }

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