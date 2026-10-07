using Microsoft.AspNetCore.Mvc;
using wpfw_opdracht3.DTOs;
using wpfw_opdracht3.Services;

namespace wpfw_opdracht3.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController : ControllerBase
{
	private readonly IProjectService _projectService;

	public ProjectsController(IProjectService projectService)
	{
		_projectService = projectService;
	}

	// GET: api/projects
	[HttpGet]
	public ActionResult<List<ProjectDto>> GetAll()
	{
		return Ok(_projectService.GetAll());
	}

	// GET: api/projects/5
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

	// POST: api/projects
	[HttpPost]
	public ActionResult<ProjectDto> Create(ProjectCreateDto dto)
	{
		var created = _projectService.Create(dto);
		return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
	}

	// PUT: api/projects/5
	[HttpPut("{id}")]
	public IActionResult Update(int id, ProjectCreateDto dto)
	{
		var success = _projectService.Update(id, dto);
		if (!success)
		{
			return NotFound();
		}
		return NoContent();
	}

	// DELETE: api/projects/5
	[HttpDelete("{id}")]
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