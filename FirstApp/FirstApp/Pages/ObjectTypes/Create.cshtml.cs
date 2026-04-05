using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FirstApp.Data;
using FirstApp.Models;

namespace FirstApp.Pages.ObjectTypes
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(int? repositoryId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Forbid();
            }

            // Initialize ObjectType with default values for audit fields
            ObjectType = new ObjectType
            {
                Name = string.Empty,
                Description = string.Empty,
                CreatedById = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedById = userId,
                UpdatedAt = DateTime.UtcNow,
                RepositoryId = repositoryId ?? 0,
                CreatedBy = null!,
                UpdatedBy = null!,
                Repository = null!
            };

            return Page();
        }

        [BindProperty]
        public ObjectType ObjectType { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId))
            {
                return Forbid();
            }

            // Set audit fields before validation
            ObjectType.CreatedById = userId;
            ObjectType.CreatedAt = DateTime.UtcNow;
            ObjectType.UpdatedById = userId;
            ObjectType.UpdatedAt = DateTime.UtcNow;

            // Remove validation errors for navigation properties since they're not bound from the form
            ModelState.Remove("ObjectType.CreatedBy");
            ModelState.Remove("ObjectType.UpdatedBy");
            ModelState.Remove("ObjectType.Repository");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ObjectTypes.Add(ObjectType);
            await _context.SaveChangesAsync();

            // If ObjectType was created from a Repository, redirect back to that Repository's details
            if (ObjectType.RepositoryId > 0)
            {
                return RedirectToPage("/Repositories/Details", new { id = ObjectType.RepositoryId });
            }

            return RedirectToPage("./Index");
        }
    }
}
