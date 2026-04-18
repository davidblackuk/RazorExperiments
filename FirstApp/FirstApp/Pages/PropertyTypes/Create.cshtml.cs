using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FirstApp.Data;
using FirstApp.Models;

namespace FirstApp.Pages.PropertyTypes
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int? objectTypeId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Forbid();
            }

            if (!objectTypeId.HasValue || objectTypeId.Value <= 0)
            {
                return NotFound();
            }

            // Load ObjectType with Repository for breadcrumb
            var objectType = await _context.ObjectTypes
                .Include(o => o.Repository)
                .FirstOrDefaultAsync(o => o.Id == objectTypeId.Value);

            if (objectType == null)
            {
                return NotFound();
            }

            var now = DateTime.UtcNow;

            // Initialize PropertyType with default values for audit fields
            PropertyType = new PropertyType
            {
                Name = string.Empty,
                Description = string.Empty,
                CreatedById = userId,
                CreatedAt = now,
                UpdatedById = userId,
                UpdatedAt = now,
                ObjectTypeId = objectTypeId.Value,
                CreatedBy = null!,
                UpdatedBy = null!,
                ObjectType = objectType
            };

            return Page();
        }

        [BindProperty]
        public PropertyType PropertyType { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId))
            {
                return Forbid();
            }

            // Set audit fields before validation
            PropertyType.CreatedById = userId;
            PropertyType.CreatedAt = DateTime.UtcNow;
            PropertyType.UpdatedById = userId;
            PropertyType.UpdatedAt = DateTime.UtcNow;

            // Remove validation errors for navigation properties since they're not bound from the form
            ModelState.Remove("PropertyType.CreatedBy");
            ModelState.Remove("PropertyType.UpdatedBy");
            ModelState.Remove("PropertyType.ObjectType");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.PropertyTypes.Add(PropertyType);
            await _context.SaveChangesAsync();

            // If PropertyType was created from an ObjectType, redirect back to that ObjectType's details
            if (PropertyType.ObjectTypeId > 0)
            {
                return RedirectToPage("/ObjectTypes/Details", new { id = PropertyType.ObjectTypeId });
            }

            return RedirectToPage("./Index");
        }
    }
}
