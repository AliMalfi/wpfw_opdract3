using Microsoft.AspNetCore.Authorization;   // voor [Authorize]
using Microsoft.AspNetCore.Mvc;
using wpfw_opdracht3.Auth;                   // voor Policies.CanEdit
using wpfw_opdracht3.DTOs;
using wpfw_opdracht3.Services;

namespace wpfw_opdracht3.Controllers;

// Controller = de "ober": ontvangt requests en geeft statuscodes terug
[Route("api/[controller]")]   // adres: api/blogposts
[ApiController]
public class BlogpostsController : ControllerBase
{
    private readonly IBlogpostService _blogpostService;

    // Service komt binnen via Dependency Injection
    public BlogpostsController(IBlogpostService blogpostService)
    {
        _blogpostService = blogpostService;
    }

    // GET: api/blogposts -> iedereen mag (publiek)
    [HttpGet]
    public ActionResult<List<BlogpostDto>> GetAll()
    {
        return Ok(_blogpostService.GetAll());
    }

    // GET: api/blogposts/5 -> iedereen mag (publiek)
    [HttpGet("{id}")]
    public ActionResult<BlogpostDto> GetById(int id)
    {
        var blogpost = _blogpostService.GetById(id);
        if (blogpost is null)
        {
            return NotFound();              // 404: bestaat niet
        }
        return Ok(blogpost);                // 200
    }

    // POST: api/blogposts -> alleen Editor of Admin
    [HttpPost]
    [Authorize(Policy = Policies.CanEdit)]
    public ActionResult<BlogpostDto> Create(BlogpostCreateDto dto)
    {
        var created = _blogpostService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);   // 201
    }

    // PUT: api/blogposts/5 -> alleen Editor of Admin
    [HttpPut("{id}")]
    [Authorize(Policy = Policies.CanEdit)]
    public IActionResult Update(int id, BlogpostCreateDto dto)
    {
        var success = _blogpostService.Update(id, dto);
        if (!success)
        {
            return NotFound();              // 404
        }
        return NoContent();                 // 204
    }

    // DELETE: api/blogposts/5 -> alleen Editor of Admin
    [HttpDelete("{id}")]
    [Authorize(Policy = Policies.CanEdit)]
    public IActionResult Delete(int id)
    {
        var success = _blogpostService.Delete(id);
        if (!success)
        {
            return NotFound();              // 404
        }
        return NoContent();                 // 204
    }
}