using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FirstApp.Data;
using FirstApp.Models;

namespace FirstApp.Pages.PropertyTypes
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
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

            var propertyType = await _context.PropertyTypes
                .Include(p => p.ObjectType)
                    .ThenInclude(o => o.Repository)
                .Include(p => p.CreatedBy)
                .Include(p => p.UpdatedBy)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (propertyType == null)
            {
                return NotFound();
            }

            PropertyType = propertyType;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyType = await _context.PropertyTypes.FindAsync(id);

            if (propertyType != null)
            {
                var objectTypeId = propertyType.ObjectTypeId;
                PropertyType = propertyType;
                _context.PropertyTypes.Remove(PropertyType);
                await _context.SaveChangesAsync();
                return RedirectToPage("/ObjectTypes/ObjectTypeDetails", new { id = objectTypeId });
            }

            return RedirectToPage("./Index");
        }
    }
}
