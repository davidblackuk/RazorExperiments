using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FirstApp.Data;
using FirstApp.Models;

namespace FirstApp.Pages.ObjectTypes
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ObjectType ObjectType { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objectType = await _context.ObjectTypes
                .Include(o => o.Repository)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (objectType == null)
            {
                return NotFound();
            }

            ObjectType = objectType;
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
            var existingObjectType = await _context.ObjectTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == ObjectType.Id);

            if (existingObjectType == null)
            {
                return NotFound();
            }

            // Update only the fields that should change
            ObjectType.UpdatedById = userId;
            ObjectType.UpdatedAt = DateTime.UtcNow;

            // Remove validation errors for navigation properties since they're not bound from the form
            ModelState.Remove("ObjectType.CreatedBy");
            ModelState.Remove("ObjectType.UpdatedBy");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(ObjectType).State = EntityState.Modified;
            // Don't modify the navigation properties
            _context.Entry(ObjectType).Reference(o => o.CreatedBy).IsModified = false;
            _context.Entry(ObjectType).Reference(o => o.UpdatedBy).IsModified = false;
            _context.Entry(ObjectType).Reference(o => o.Repository).IsModified = false;

            await _context.SaveChangesAsync();

            return RedirectToPage("/Designer/Details", new { id = ObjectType.RepositoryId });
        }

        private bool ObjectTypeExists(int id)
        {
            return _context.ObjectTypes.Any(e => e.Id == id);
        }
    }
}
