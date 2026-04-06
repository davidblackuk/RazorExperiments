using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FirstApp.Data;
using FirstApp.Models;

namespace FirstApp.Pages.PropertyTypes
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PropertyType PropertyType { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyType = await _context.PropertyTypes.FirstOrDefaultAsync(m => m.Id == id);
            
            if (propertyType == null)
            {
                return NotFound();
            }
            
            PropertyType = propertyType;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Forbid();
            }

            // Load the existing entity from the database
            var existingPropertyType = await _context.PropertyTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == PropertyType.Id);

            if (existingPropertyType == null)
            {
                return NotFound();
            }

            // Update only the fields that should change
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

            _context.Attach(PropertyType).State = EntityState.Modified;
            // Don't modify the navigation properties
            _context.Entry(PropertyType).Reference(p => p.CreatedBy).IsModified = false;
            _context.Entry(PropertyType).Reference(p => p.UpdatedBy).IsModified = false;
            _context.Entry(PropertyType).Reference(p => p.ObjectType).IsModified = false;

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private bool PropertyTypeExists(int id)
        {
            return _context.PropertyTypes.Any(e => e.Id == id);
        }
    }
}
