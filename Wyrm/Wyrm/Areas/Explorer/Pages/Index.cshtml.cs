using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Wyrm.Data;
using Wyrm.Models;

namespace Wyrm.Areas.Explorer.Pages
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
                .ToListAsync();
        }
    }
}
