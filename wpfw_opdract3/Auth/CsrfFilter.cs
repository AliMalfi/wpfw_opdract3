using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace wpfw_opdracht3.Auth;

// Controleert bij elke POST, PUT en DELETE of er een geldig CSRF-token is meegestuurd
public class CsrfFilter(IAntiforgery antiforgery) : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var methode = context.HttpContext.Request.Method;

        // 1. Lezen (GET, HEAD, OPTIONS) verandert niets → geen token nodig
        if (HttpMethods.IsGet(methode) || HttpMethods.IsHead(methode) || HttpMethods.IsOptions(methode))
            return;

        // 2. Schrijven: token (header) en cookie moeten bij elkaar passen
        if (!await antiforgery.IsRequestValidAsync(context.HttpContext))
        {
            // 3. Geen of fout token → 400, de actie wordt NIET uitgevoerd
            context.Result = new BadRequestObjectResult(new { message = "Ongeldig of ontbrekend CSRF-token." });
        }
    }
}