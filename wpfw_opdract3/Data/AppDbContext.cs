using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using wpfw_opdracht3.Models;

namespace wpfw_opdracht3.Data;

// IdentityDbContext = gewone DbContext + tabellen voor gebruikers en rollen
// IDataProtectionKeyContext = ook een tabel voor de sleutels van de inlog-cookies
public class AppDbContext : IdentityDbContext<User>, IDataProtectionKeyContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Project> Projects { get; set; }
    public DbSet<Blogpost> Blogposts { get; set; }

    // NIEUW: hier worden de sleutels bewaard, zodat cookies geldig blijven na een nieuwe container
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);                  // eerst Identity instellen
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