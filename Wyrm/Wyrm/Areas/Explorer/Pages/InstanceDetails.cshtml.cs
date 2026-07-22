using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Wyrm.Data;
using Wyrm.Models;
using Wyrm.Services;

namespace Wyrm.Areas.Explorer.Pages
{
    public class InstanceDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public InstanceDetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public ObjectInstance ObjectInstance { get; set; } = default!;

        public string DisplayName { get; set; } = string.Empty;

        public Dictionary<int, string?> Values { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instance = await _context.ObjectInstances
                .Include(i => i.ObjectType!.Repository)
                .Include(i => i.ObjectType!.PropertyTypes.OrderBy(pt => pt.Id))
                .Include(i => i.CreatedBy)
                .Include(i => i.UpdatedBy)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (instance == null)
            {
                return NotFound();
            }

            ObjectInstance = instance;
            Values = await PropertyValueStore.LoadRawValuesAsync(_context, instance.Id, instance.ObjectType!.PropertyTypes);
            DisplayName = await InstanceDisplayHelper.GetDisplayNameAsync(_context, instance);

            return Page();
        }
    }
}
