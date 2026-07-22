using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Wyrm.Data;
using Wyrm.Models;
using Wyrm.Services;

namespace Wyrm.Areas.Explorer.Pages
{
    public class DeleteInstanceModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteInstanceModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ObjectInstance ObjectInstance { get; set; } = default!;

        public string DisplayName { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instance = await _context.ObjectInstances
                .Include(i => i.ObjectType!.Repository)
                .Include(i => i.CreatedBy)
                .Include(i => i.UpdatedBy)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (instance == null)
            {
                return NotFound();
            }

            ObjectInstance = instance;
            DisplayName = await InstanceDisplayHelper.GetDisplayNameAsync(_context, instance);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instance = await _context.ObjectInstances.FindAsync(id);

            if (instance != null)
            {
                var objectTypeId = instance.ObjectTypeId;
                ObjectInstance = instance;
                _context.ObjectInstances.Remove(ObjectInstance);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Instances", new { objectTypeId });
            }

            return RedirectToPage("./Index");
        }
    }
}
