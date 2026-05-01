using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FirstApp.Data;
using FirstApp.Models;

namespace FirstApp.Pages.Designer
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Forbid();
            }

            // Initialize Repository with default values for audit fields
            Repository = new Repository
            {
                Name = string.Empty,
                Description = string.Empty,
                CreatedById = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedById = userId,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = null!,
                UpdatedBy = null!
            };

            return Page();
        }

        [BindProperty]
        public Repository Repository { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Forbid();
            }

            // Set audit fields before validation
            Repository.CreatedById = userId;
            Repository.CreatedAt = DateTime.UtcNow;
            Repository.UpdatedById = userId;
            Repository.UpdatedAt = DateTime.UtcNow;

            // Remove validation errors for navigation properties since they're not bound from the form
            ModelState.Remove("Repository.CreatedBy");
            ModelState.Remove("Repository.UpdatedBy");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Repositories.Add(Repository);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
