using wpfw_opdracht3.DTOs;

namespace wpfw_opdracht3.Services;

public interface IBlogpostService
{
    List<BlogpostDto> GetAll();
    BlogpostDto? GetById(int id);
}