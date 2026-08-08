using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Wyrm.Components.Account.Pages;

public partial class ResetPassword : ComponentBase
{
    private bool invalidCode;
    private string? errorMessage;

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    [SupplyParameterFromQuery]
    private string? Code { get; set; }

    protected override void OnInitialized()
    {
        if (Code is null)
        {
            invalidCode = true;
            return;
        }

        Input.Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));
    }

    private async Task ResetPasswordUser()
    {
        var user = await UserManager.FindByEmailAsync(Input.Email);
        if (user is null)
        {
            // Don't reveal that the user does not exist
            RedirectManager.RedirectTo("Account/ResetPasswordConfirmation");
            return;
        }

        var result = await UserManager.ResetPasswordAsync(user, Input.Code, Input.Password);
        if (result.Succeeded)
        {
            RedirectManager.RedirectTo("Account/ResetPasswordConfirmation");
            return;
        }

        errorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
    }

    private sealed class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = "";

        public string Code { get; set; } = "";
    }
}
