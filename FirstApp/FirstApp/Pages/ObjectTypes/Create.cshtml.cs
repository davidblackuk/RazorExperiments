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

        public IActionResult OnGet()
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
                CreatedBy = null!,
                UpdatedBy = null!
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

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ObjectTypes.Add(ObjectType);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
