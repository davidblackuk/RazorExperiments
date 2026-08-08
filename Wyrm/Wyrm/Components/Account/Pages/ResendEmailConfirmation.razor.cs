using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Wyrm.Components.Account.Pages;

public partial class ResendEmailConfirmation : ComponentBase
{
    private string? message;

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    private async Task ResendConfirmation()
    {
        var user = await UserManager.FindByEmailAsync(Input.Email);
        if (user is not null)
        {
            var userId = await UserManager.GetUserIdAsync(user);
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = NavigationManager.GetUriWithQueryParameters(
                NavigationManager.ToAbsoluteUri("Account/ConfirmEmail").ToString(),
                new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code });

            await EmailSender.SendConfirmationLinkAsync(user, Input.Email, callbackUrl);
        }

        // Deliberately not revealing whether the address is registered (matches the original scaffold)
        message = "Verification email sent. Please check your email.";
    }

    private sealed class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}
