using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FirstApp.Data;
using FirstApp.Models;

namespace FirstApp.Pages.Repositories
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Repository Repository { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var repository = await _context.Repositories
                .Include(r => r.CreatedBy)
                .Include(r => r.UpdatedBy)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (repository == null)
            {
                return NotFound();
            }

            Repository = repository;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var repository = await _context.Repositories.FindAsync(id);
            
            if (repository != null)
            {
                Repository = repository;
                _context.Repositories.Remove(Repository);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
