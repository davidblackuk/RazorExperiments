using Microsoft.AspNetCore.Components;

namespace Wyrm.Components.Account.Shared;

public partial class LoginDisplay : ComponentBase
{
    private string currentPath = "/";

    protected override void OnInitialized()
    {
        currentPath = "/" + NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
    }
}
