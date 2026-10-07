using Microsoft.AspNetCore.Mvc;
using wpfw_opdracht3.DTOs;
using wpfw_opdracht3.Services;

namespace wpfw_opdracht3.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BlogpostsController : ControllerBase
{
    private readonly IBlogpostService _blogpostService;

    public BlogpostsController(IBlogpostService blogpostService)
    {
        _blogpostService = blogpostService;
    }

    // GET: api/blogposts
    [HttpGet]
    public ActionResult<List<BlogpostDto>> GetAll()
    {
        return Ok(_blogpostService.GetAll());
    }

    // GET: api/blogposts/5
    [HttpGet("{id}")]
    public ActionResult<BlogpostDto> GetById(int id)
    {
        var blogpost = _blogpostService.GetById(id);
        if (blogpost is null)
        {
            return NotFound();
        }
        return Ok(blogpost);
    }
}