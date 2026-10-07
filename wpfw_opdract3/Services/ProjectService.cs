using Microsoft.EntityFrameworkCore;
using wpfw_opdracht3.Data;
using wpfw_opdracht3.DTOs;
using wpfw_opdracht3.Models;

namespace wpfw_opdracht3.Services;

public class ProjectService : IProjectService
{
	private readonly AppDbContext _context;

	public ProjectService(AppDbContext context)
	{
		_context = context;
	}

	public List<ProjectDto> GetAll()
	{
		return _context.Projects
			.Select(p => ToDto(p))
			.ToList();
	}

	public ProjectDto? GetById(int id)
	{
		var project = _context.Projects.Find(id);
		return project is null ? null : ToDto(project);
	}

	public ProjectDto Create(ProjectCreateDto dto)
	{
		var project = new Project
		{
			Titel = dto.Titel,
			Beschrijving = dto.Beschrijving,
			Categorie = dto.Categorie,
			GithubUrl = dto.GithubUrl,
			Datum = dto.Datum
		};

		_context.Projects.Add(project);
		_context.SaveChanges();

		return ToDto(project);
	}

	public bool Update(int id, ProjectCreateDto dto)
	{
		var project = _context.Projects.Find(id);
		if (project is null) return false;

		project.Titel = dto.Titel;
		project.Beschrijving = dto.Beschrijving;
		project.Categorie = dto.Categorie;
		project.GithubUrl = dto.GithubUrl;
		project.Datum = dto.Datum;

		_context.SaveChanges();
		return true;
	}

	public bool Delete(int id)
	{
		var project = _context.Projects.Find(id);
		if (project is null) return false;

		_context.Projects.Remove(project);
		_context.SaveChanges();
		return true;
	}

	private static ProjectDto ToDto(Project p)
	{
		return new ProjectDto
		{
			Id = p.Id,
			Titel = p.Titel,
			Beschrijving = p.Beschrijving,
			Categorie = p.Categorie,
			GithubUrl = p.GithubUrl,
			Datum = p.Datum
		};
	}
}