using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FirstApp.Data;
using FirstApp.Models;

namespace FirstApp.Pages.Designer
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Repository> Repositories { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Repositories = await _context.Repositories
                .Include(r => r.ObjectTypes)
                .Include(r => r.CreatedBy)
                .Include(r => r.UpdatedBy)
                .ToListAsync();
        }
    }
}
