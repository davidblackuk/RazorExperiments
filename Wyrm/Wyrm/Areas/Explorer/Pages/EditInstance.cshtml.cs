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
    public class EditInstanceModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditInstanceModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public ObjectType ObjectType { get; set; } = default!;

        public string DisplayName { get; set; } = string.Empty;

        [BindProperty]
        public int InstanceId { get; set; }

        [BindProperty]
        public List<PropertyFieldInput> Fields { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Forbid();
            }

            if (id == null)
            {
                return NotFound();
            }

            var instance = await _context.ObjectInstances
                .Include(i => i.ObjectType!.Repository)
                .Include(i => i.ObjectType!.PropertyTypes.OrderBy(pt => pt.Id))
                .FirstOrDefaultAsync(i => i.Id == id);

            if (instance == null)
            {
                return NotFound();
            }

            ObjectType = instance.ObjectType!;
            InstanceId = instance.Id;
            DisplayName = await InstanceDisplayHelper.GetDisplayNameAsync(_context, instance);

            var editableTypes = EditableFields(ObjectType.PropertyTypes);
            var existingValues = await PropertyValueStore.LoadRawValuesAsync(_context, instance.Id, editableTypes);
            Fields = BuildFields(editableTypes, pt => existingValues.GetValueOrDefault(pt.Id));

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Forbid();
            }

            var instance = await _context.ObjectInstances
                .Include(i => i.ObjectType!.Repository)
                .Include(i => i.ObjectType!.PropertyTypes.OrderBy(pt => pt.Id))
                .FirstOrDefaultAsync(i => i.Id == InstanceId);

            if (instance == null)
            {
                return NotFound();
            }

            ObjectType = instance.ObjectType!;
            DisplayName = await InstanceDisplayHelper.GetDisplayNameAsync(_context, instance);

            var editableTypes = EditableFields(ObjectType.PropertyTypes);
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

            Fields = BuildFields(editableTypes, pt => postedRawValues.GetValueOrDefault(pt.Id));

            if (hasError)
            {
                return Page();
            }

            var now = DateTime.UtcNow;
            instance.UpdatedById = userId;
            instance.UpdatedAt = now;

            foreach (var propertyType in editableTypes)
            {
                postedRawValues.TryGetValue(propertyType.Id, out var rawValue);
                await PropertyValueStore.SetValueAsync(_context, instance, propertyType, rawValue, userId, now);
            }

            var user = await _context.Users.FindAsync(userId);
            await PropertyValueStore.SetAuditMirrorValuesAsync(_context, instance, ObjectType.PropertyTypes, user?.UserName ?? userId, userId, now, isCreate: false);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Instances", new { objectTypeId = ObjectType.Id });
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
