using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using wpfw_opdracht3.Auth;

namespace wpfw_opdracht3.Data;

// Zorgt dat de database bijgewerkt is en de rollen Gast, Editor en Admin bestaan
public static class RoleSeeder
{
    public static async Task SeedRolesAsync(IServiceProvider services)
    {
        // 1. Tijdelijke "werkplek" maken om services op te halen
        using var scope = services.CreateScope();

        // 2. NIEUW: database bijwerken met alle migraties (maakt tabellen aan als ze ontbreken)
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        // 3. RoleManager = hulp van Identity om rollen te beheren
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // 4. Voor elke rol: bestaat hij nog niet? Dan aanmaken
        string[] alleRollen = { Roles.Gast, Roles.Editor, Roles.Admin };
        foreach (var rol in alleRollen)
        {
            if (!await roleManager.RoleExistsAsync(rol))
            {
                await roleManager.CreateAsync(new IdentityRole(rol));
            }
        }
    }
}