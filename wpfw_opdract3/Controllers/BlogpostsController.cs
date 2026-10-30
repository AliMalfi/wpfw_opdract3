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
    [HttpPost]
    public ActionResult<BlogpostDto> Create(BlogpostCreateDto dto)
    {
        var blogpost = _blogpostService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = blogpost.Id }, blogpost);
    }
    [HttpPut("{id}")]
    public IActionResult Update(int id, BlogpostCreateDto dto)
    {
        var success = _blogpostService.Update(id, dto);
        if (!success)
        {
            return NotFound();            
        }
        return NoContent();                 
    }

    // DELETE: api/blogposts/5 -> verwijderen (204) of 404
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var success = _blogpostService.Delete(id);
        if (!success)
        {
            return NotFound();             
        }
        return NoContent();                 
    }
}