using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FirstApp.Data;
using FirstApp.Models;

namespace FirstApp.Areas.Explorer.Pages
{
    public class ObjectTypesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ObjectTypesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Repository Repository { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? repositoryId)
        {
            if (repositoryId == null)
            {
                return NotFound();
            }

            var repository = await _context.Repositories
                .Include(r => r.ObjectTypes)
                    .ThenInclude(ot => ot.PropertyTypes)
                .FirstOrDefaultAsync(r => r.Id == repositoryId);

            if (repository == null)
            {
                return NotFound();
            }

            Repository = repository;
            return Page();
        }
    }
}
