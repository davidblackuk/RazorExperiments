using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FirstApp.Data;
using FirstApp.Models;

namespace FirstApp.Pages.ObjectTypes
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ObjectType> ObjectType { get; set; } = default!;

        public async Task OnGetAsync()
        {
            ObjectType = await _context.ObjectTypes
                .Include(o => o.Repository)
                .Include(o => o.CreatedBy)
                .Include(o => o.UpdatedBy)
                .ToListAsync();
        }
    }
}
