using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Wyrm.Data;
using Wyrm.Models;
using Wyrm.Services;

namespace Wyrm.Areas.Explorer.Pages
{
    public class InstancesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public InstancesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public ObjectType ObjectType { get; set; } = default!;

        public Dictionary<int, string> DisplayNames { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? objectTypeId)
        {
            if (!objectTypeId.HasValue || objectTypeId.Value <= 0)
            {
                return NotFound();
            }

            var objectType = await _context.ObjectTypes
                .Include(o => o.Repository)
                .Include(o => o.ObjectInstances)
                .FirstOrDefaultAsync(o => o.Id == objectTypeId.Value);

            if (objectType == null)
            {
                return NotFound();
            }

            ObjectType = objectType;
            DisplayNames = await InstanceDisplayHelper.GetDisplayNamesAsync(
                _context,
                objectType.Id,
                objectType.ObjectInstances.Select(i => i.Id));

            return Page();
        }
    }
}
