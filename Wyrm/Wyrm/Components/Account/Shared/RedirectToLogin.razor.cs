using Microsoft.AspNetCore.Components;

namespace Wyrm.Components.Account.Shared;

public partial class RedirectToLogin : ComponentBase
{
    protected override void OnInitialized()
    {
        NavigationManager.NavigateTo(
            $"Account/Login?returnUrl={Uri.EscapeDataString(NavigationManager.Uri)}",
            forceLoad: true);
    }
}
