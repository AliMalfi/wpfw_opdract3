namespace wpfw_opdracht3;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using wpfw_opdracht3.Auth;
using wpfw_opdracht3.Data;
using wpfw_opdracht3.Models;
using wpfw_opdracht3.Services;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Lokaal (Development) mag http, online (Production) ALLEEN https
        var cookieBeleid = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;

        // 1. Connection string en DbContext registreren
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // 2. NIEUW: sleutels voor cookies in de DATABASE bewaren (niet in de container)
        //    -> na een nieuwe versie blijft iedereen ingelogd
        builder.Services.AddDataProtection()
            .PersistKeysToDbContext<AppDbContext>()
            .SetApplicationName("Portfolio");

        // 3. Identity aanzetten (gebruikers + rollen, opgeslagen in onze database)
        builder.Services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();

        // 4. Policy: alleen Editor of Admin mag schrijven (POST, PUT, DELETE)
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(Policies.CanEdit, policy =>
                policy.RequireRole(Roles.Editor, Roles.Admin));

        // 5. Inlog-cookie veilig maken + 401/403 in plaats van doorsturen
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;                 // JavaScript kan de cookie niet lezen
            options.Cookie.SecurePolicy = cookieBeleid;     // online alleen via https
            options.Cookie.SameSite = SameSiteMode.Strict;  // niet meesturen vanaf andere websites

            // Niet ingelogd -> 401
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };

            // Ingelogd, maar geen Editor of Admin -> 403
            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        });

        // 6. CSRF-bescherming: token komt in de header X-CSRF-TOKEN
        builder.Services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
            options.Cookie.Name = "portfolio.csrf";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = cookieBeleid;
            options.Cookie.SameSite = SameSiteMode.Strict;
        });

        // 7. Achter Nginx/Azure: de echte "https" en het IP-adres overnemen
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();   // de proxy draait in een andere container
            options.KnownProxies.Clear();
        });

        // 8. Services via Dependency Injection (interface -> implementatie, per request)
        builder.Services.AddScoped<IProjectService, ProjectService>();
        builder.Services.AddScoped<IBlogpostService, BlogpostService>();

        // 9. Controllers + CSRF-filter op ALLE controllers
        builder.Services.AddControllers(options => options.Filters.Add<CsrfFilter>());
        builder.Services.AddRouting();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseForwardedHeaders();   // als EERSTE: weet dat de bezoeker https gebruikt

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;   // Swagger op de startpagina (/)
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();   // 1. Wie ben jij? (leest de cookie)
        app.UseAuthorization();    // 2. Wat mag jij?
        app.MapControllers();

        // Rollen Gast, Editor en Admin aanmaken (en migraties uitvoeren)
        await RoleSeeder.SeedRolesAsync(app.Services);

        app.Run();
    }
}