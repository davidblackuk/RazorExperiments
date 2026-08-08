using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Wyrm.Components.Account.Pages;

public partial class RegisterConfirmation : ComponentBase
{
    private bool displayConfirmAccountLink;
    private bool userNotFound;
    private string? emailConfirmationUrl;

    [SupplyParameterFromQuery]
    private string? Email { get; set; }

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Email is null)
        {
            RedirectManager.RedirectTo("/");
            return;
        }

        var user = await UserManager.FindByEmailAsync(Email);
        if (user is null)
        {
            userNotFound = true;
            return;
        }

        // Once you add a real email sender, you should remove this code that lets you confirm the account
        displayConfirmAccountLink = true;

        var userId = await UserManager.GetUserIdAsync(user);
        var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        emailConfirmationUrl = NavigationManager.GetUriWithQueryParameters(
            NavigationManager.ToAbsoluteUri("Account/ConfirmEmail").ToString(),
            new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code, ["returnUrl"] = ReturnUrl });
    }
}
