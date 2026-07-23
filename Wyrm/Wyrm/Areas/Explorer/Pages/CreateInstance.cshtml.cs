using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Wyrm.Data;
using Wyrm.Models;
using Wyrm.Services;
using Wyrm.ViewModels;

namespace Wyrm.Areas.Explorer.Pages
{
    public class CreateInstanceModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateInstanceModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public ObjectType ObjectType { get; set; } = default!;

        [BindProperty]
        public int ObjectTypeId { get; set; }

        [BindProperty]
        public List<PropertyFieldInput> Fields { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? objectTypeId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Forbid();
            }

            if (!objectTypeId.HasValue || objectTypeId.Value <= 0)
            {
                return NotFound();
            }

            var objectType = await _context.ObjectTypes
                .Include(o => o.Repository)
                .Include(o => o.PropertyTypes.OrderBy(pt => pt.Id))
                .FirstOrDefaultAsync(o => o.Id == objectTypeId.Value);

            if (objectType == null)
            {
                return NotFound();
            }

            ObjectType = objectType;
            ObjectTypeId = objectType.Id;
            Fields = BuildFields(EditableFields(objectType.PropertyTypes), rawValue: _ => null);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Forbid();
            }

            var objectType = await _context.ObjectTypes
                .Include(o => o.Repository)
                .Include(o => o.PropertyTypes.OrderBy(pt => pt.Id))
                .FirstOrDefaultAsync(o => o.Id == ObjectTypeId);

            if (objectType == null)
            {
                return NotFound();
            }

            ObjectType = objectType;

            var editableTypes = EditableFields(objectType.PropertyTypes);
            var postedRawValues = Fields.ToDictionary(f => f.PropertyTypeId, f => f.RawValue);

            var hasError = false;
            foreach (var propertyType in editableTypes)
            {
                postedRawValues.TryGetValue(propertyType.Id, out var rawValue);
                if (!PropertyValueParser.TryValidate(propertyType.DataType, rawValue, out var error))
                {
                    ModelState.AddModelError(string.Empty, $"{propertyType.Name}: {error}");
                    hasError = true;
                }
            }

            // Metadata doesn't round-trip through POST (only PropertyTypeId + RawValue do),
            // so rebuild Fields from the freshly-loaded PropertyTypes for redisplay on error.
            Fields = BuildFields(editableTypes, rawValue: pt => postedRawValues.GetValueOrDefault(pt.Id));

            if (hasError)
            {
                return Page();
            }

            var now = DateTime.UtcNow;
            var instance = new ObjectInstance
            {
                ObjectTypeId = objectType.Id,
                CreatedById = userId,
                CreatedAt = now,
                UpdatedById = userId,
                UpdatedAt = now
            };

            _context.ObjectInstances.Add(instance);
            await _context.SaveChangesAsync();

            foreach (var propertyType in editableTypes)
            {
                postedRawValues.TryGetValue(propertyType.Id, out var rawValue);
                await PropertyValueStore.SetValueAsync(_context, instance, propertyType, rawValue, userId, now);
            }

            var user = await _context.Users.FindAsync(userId);
            await PropertyValueStore.SetAuditMirrorValuesAsync(_context, instance, objectType.PropertyTypes, user?.UserName ?? userId, userId, now, isCreate: true);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Instances", new { objectTypeId = objectType.Id });
        }

        private static IReadOnlyList<PropertyType> EditableFields(IEnumerable<PropertyType> propertyTypes)
        {
            return propertyTypes.Where(pt => !SystemPropertyNames.IsAuditMirror(pt.Name)).ToList();
        }

        private static List<PropertyFieldInput> BuildFields(IEnumerable<PropertyType> propertyTypes, Func<PropertyType, string?> rawValue)
        {
            return propertyTypes.Select(pt => new PropertyFieldInput
            {
                PropertyTypeId = pt.Id,
                Name = pt.Name,
                Description = pt.Description,
                DataType = pt.DataType,
                RawValue = rawValue(pt)
            }).ToList();
        }
    }
}
