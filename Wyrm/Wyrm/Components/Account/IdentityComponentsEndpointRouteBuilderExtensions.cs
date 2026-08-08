using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Wyrm.Components.Account;

internal static class IdentityComponentsEndpointRouteBuilderExtensions
{
    // Identity operations that must write authentication cookies can't run inside an interactive
    // Blazor circuit (the response has already started streaming by then), so logout is a plain
    // minimal API endpoint invoked by a normal HTML form POST instead of a routable component.
    public static IEndpointRouteBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var accountGroup = endpoints.MapGroup("/Account");

        accountGroup.MapPost("/Logout", async (
            SignInManager<IdentityUser> signInManager,
            [FromForm] string? returnUrl) =>
        {
            await signInManager.SignOutAsync();
            return TypedResults.LocalRedirect(string.IsNullOrEmpty(returnUrl) ? "~/" : returnUrl);
        });

        return endpoints;
    }
}
