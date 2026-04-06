using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FirstApp.Data;
using FirstApp.Models;

namespace FirstApp.Pages.PropertyTypes
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<PropertyType> PropertyType { get; set; } = default!;

        public async Task OnGetAsync()
        {
            PropertyType = await _context.PropertyTypes
                .Include(p => p.ObjectType)
                .Include(p => p.CreatedBy)
                .Include(p => p.UpdatedBy)
                .ToListAsync();
        }
    }
}
