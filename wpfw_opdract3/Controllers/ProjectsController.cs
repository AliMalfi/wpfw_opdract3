using Microsoft.AspNetCore.Authorization;   // NIEUW: voor [Authorize]
using Microsoft.AspNetCore.Mvc;
using wpfw_opdracht3.Auth;                   // NIEUW: voor Policies.CanEdit
using wpfw_opdracht3.DTOs;
using wpfw_opdracht3.Services;

namespace wpfw_opdracht3.Controllers;

// Controller = de "ober": ontvangt requests en geeft statuscodes terug
[Route("api/[controller]")]   // adres: api/projects
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    // Service komt binnen via Dependency Injection
    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    // GET: api/projects -> iedereen mag (publiek)
    [HttpGet]
    public ActionResult<List<ProjectDto>> GetAll()
    {
        return Ok(_projectService.GetAll());
    }

    // GET: api/projects/5 -> iedereen mag (publiek)
    [HttpGet("{id}")]
    public ActionResult<ProjectDto> GetById(int id)
    {
        var project = _projectService.GetById(id);
        if (project is null)
        {
            return NotFound();
        }
        return Ok(project);
    }

    // POST: api/projects -> alleen Editor of Admin
    [HttpPost]
    [Authorize(Policy = Policies.CanEdit)]   // NIEUW: bordje
    public ActionResult<ProjectDto> Create(ProjectCreateDto dto)
    {
        var created = _projectService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT: api/projects/5 -> alleen Editor of Admin
    [HttpPut("{id}")]
    [Authorize(Policy = Policies.CanEdit)]   // NIEUW: bordje
    public IActionResult Update(int id, ProjectCreateDto dto)
    {
        var success = _projectService.Update(id, dto);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }

    // DELETE: api/projects/5 -> alleen Editor of Admin
    [HttpDelete("{id}")]
    [Authorize(Policy = Policies.CanEdit)]   // NIEUW: bordje
    public IActionResult Delete(int id)
    {
        var success = _projectService.Delete(id);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }
}