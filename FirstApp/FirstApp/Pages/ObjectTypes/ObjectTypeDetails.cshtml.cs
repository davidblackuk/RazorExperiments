using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FirstApp.Data;
using FirstApp.Models;

namespace FirstApp.Pages.ObjectTypes
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public ObjectType ObjectType { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objectType = await _context.ObjectTypes
                .Include(o => o.PropertyTypes)
                .Include(o => o.CreatedBy)
                .Include(o => o.UpdatedBy)
                .Include(o => o.Repository)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (objectType == null)
            {
                return NotFound();
            }

            ObjectType = objectType;
            return Page();
        }
    }
}
