using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Wyrm.Data;
using Wyrm.Models;
using Wyrm.Services;
using Wyrm.ViewModels;

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
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        /// <summary>
        /// AJAX handler backing the top-right instance grid: renders the instances of one ObjectType.
        /// </summary>
        public async Task<IActionResult> OnGetGridAsync(int objectTypeId)
        {
            var objectType = await _context.ObjectTypes
                .Include(o => o.Repository)
                .Include(o => o.ObjectInstances)
                    .ThenInclude(i => i.CreatedBy)
                .Include(o => o.ObjectInstances)
                    .ThenInclude(i => i.UpdatedBy)
                .FirstOrDefaultAsync(o => o.Id == objectTypeId);

            if (objectType == null)
            {
                return NotFound();
            }

            var displayNames = await InstanceDisplayHelper.GetDisplayNamesAsync(
                _context, objectType.Id, objectType.ObjectInstances.Select(i => i.Id));

            var rows = objectType.ObjectInstances
                .Select(i => new ExplorerInstanceRow
                {
                    Id = i.Id,
                    DisplayName = displayNames[i.Id],
                    CreatedByUserName = i.CreatedBy?.UserName,
                    UpdatedByUserName = i.UpdatedBy?.UserName
                })
                .OrderBy(r => r.DisplayName)
                .ToList();

            return Partial("Shared/_InstanceGridPartial", new ExplorerGridViewModel
            {
                ObjectType = objectType,
                Rows = rows
            });
        }

        /// <summary>
        /// AJAX handler backing the bottom-right detail panel: renders one instance's property values.
        /// </summary>
        public async Task<IActionResult> OnGetDetailAsync(int instanceId)
        {
            var instance = await _context.ObjectInstances
                .Include(i => i.ObjectType!.Repository)
                .Include(i => i.ObjectType!.PropertyTypes.OrderBy(pt => pt.Id))
                .Include(i => i.CreatedBy)
                .Include(i => i.UpdatedBy)
                .FirstOrDefaultAsync(i => i.Id == instanceId);

            if (instance == null)
            {
                return NotFound();
            }

            var values = await PropertyValueStore.LoadRawValuesAsync(_context, instance.Id, instance.ObjectType!.PropertyTypes);
            var displayName = await InstanceDisplayHelper.GetDisplayNameAsync(_context, instance);

            return Partial("Shared/_InstanceDetailPartial", new ExplorerInstanceDetailViewModel
            {
                ObjectInstance = instance,
                DisplayName = displayName,
                Values = values
            });
        }
    }
}
