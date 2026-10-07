using Microsoft.AspNetCore.Mvc;
using wpfw_opdracht3.Models;

namespace wpfw_opdracht3.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentController : ControllerBase
{
    // GET: api/Student
    [HttpGet]
    public string GetString()
    {
        return "Het endpoint werkt!";
    }

    // GET: api/Student/student
    [HttpGet("student")]
    public Student GetStudent()
    {
        Student student = new Student
        {
            Id = 1,
            Studentnummer = "25012345",
            Naam = "Ali"
        };
        return student;
    }



}