using Microsoft.EntityFrameworkCore;
using wpfw_opdracht3.Models;

namespace wpfw_opdracht3.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Project> Projects { get; set; }
    public DbSet<Blogpost> Blogposts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blogpost>().HasData(
            new Blogpost
            {
                Id = 1,
                Titel = "Mijn eerste stappen met React",
                Inhoud = "Deze week ben ik begonnen met React voor mijn Smart Environment Dashboard.",
                Publicatiedatum = new DateTime(2026, 9, 1)
            },
            new Blogpost
            {
                Id = 2,
                Titel = "Waarom semantische HTML ertoe doet",
                Inhoud = "Tijdens het bouwen van mijn portfolio leerde ik hoe belangrijk toegankelijkheid is.",
                Publicatiedatum = new DateTime(2026, 8, 20)
            }
        );
    }
}