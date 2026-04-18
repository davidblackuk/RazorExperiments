using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FirstApp.Data;
using FirstApp.Models;

namespace FirstApp.Pages.ObjectTypes
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
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
                .Include(o => o.CreatedBy)
                .Include(o => o.UpdatedBy)
                .Include(o => o.PropertyTypes)
                .Include(o => o.Repository)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (objectType == null)
            {
                return NotFound();
            }

            ObjectType = objectType;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objectType = await _context.ObjectTypes.FindAsync(id);

            if (objectType != null)
            {
                var repositoryId = objectType.RepositoryId;
                ObjectType = objectType;
                _context.ObjectTypes.Remove(ObjectType);
                await _context.SaveChangesAsync();
                return RedirectToPage("/Repositories/Details", new { id = repositoryId });
            }

            return RedirectToPage("./Index");
        }
    }
}
