using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace Wyrm.Components.Account;

public sealed class IdentityRedirectManager(NavigationManager navigationManager)
{
    [DoesNotReturn]
    public void RedirectTo(string uri)
    {
        // During static rendering, NavigateTo throws a NavigationException which the framework
        // handles as a redirect. This only works from a statically rendered Identity component.
        navigationManager.NavigateTo(uri);
        throw new InvalidOperationException($"{nameof(IdentityRedirectManager)} can only be used during static rendering.");
    }

    [DoesNotReturn]
    public void RedirectTo(string uri, Dictionary<string, object?> queryParameters)
    {
        var uriWithoutQuery = navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
        var newUri = navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
        RedirectTo(newUri);
    }
}
