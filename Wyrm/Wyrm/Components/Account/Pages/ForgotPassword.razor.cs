using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Wyrm.Components.Account.Pages;

public partial class ForgotPassword : ComponentBase
{
    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    private async Task RequestReset()
    {
        var user = await UserManager.FindByEmailAsync(Input.Email);
        if (user is null || !await UserManager.IsEmailConfirmedAsync(user))
        {
            // Don't reveal that the user does not exist or is not confirmed
            RedirectManager.RedirectTo("Account/ForgotPasswordConfirmation");
            return;
        }

        var code = await UserManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = NavigationManager.GetUriWithQueryParameters(
            NavigationManager.ToAbsoluteUri("Account/ResetPassword").ToString(),
            new Dictionary<string, object?> { ["code"] = code });

        await EmailSender.SendPasswordResetLinkAsync(user, Input.Email, callbackUrl);

        RedirectManager.RedirectTo("Account/ForgotPasswordConfirmation");
    }

    private sealed class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}
