namespace wpfw_opdracht3;

using Microsoft.EntityFrameworkCore;
using wpfw_opdracht3.Data;
using wpfw_opdracht3.Services;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. Connection string en DbContext registreren
        builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        // Registreer de ProjectService via Dependency Injection: koppel de interface aan de concrete implementatie per HTTP-request
        builder.Services.AddScoped<IProjectService, ProjectService>();
        //blog
        builder.Services.AddScoped<IBlogpostService, BlogpostService>();

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddRouting();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
