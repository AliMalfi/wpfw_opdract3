using wpfw_opdracht3.DTOs;

namespace wpfw_opdracht3.Services
{
    public interface IProjectService
    {
        List<ProjectDto> GetAll();
        ProjectDto? GetById(int id);
        ProjectDto Create(ProjectCreateDto dto);
        bool Update(int id, ProjectCreateDto dto);
        bool Delete(int id);
    }
}