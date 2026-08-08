using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Wyrm.Components.Account.Pages;

public partial class ConfirmEmail : ComponentBase
{
    private string? statusMessage;

    [SupplyParameterFromQuery]
    private string? UserId { get; set; }

    [SupplyParameterFromQuery]
    private string? Code { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (UserId is null || Code is null)
        {
            RedirectManager.RedirectTo("/");
            return;
        }

        var user = await UserManager.FindByIdAsync(UserId);
        if (user is null)
        {
            statusMessage = $"Error: Unable to load user with ID '{UserId}'.";
            return;
        }

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
        var result = await UserManager.ConfirmEmailAsync(user, code);
        statusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";
    }
}
