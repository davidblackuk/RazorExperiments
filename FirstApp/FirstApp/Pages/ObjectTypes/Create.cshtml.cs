using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FirstApp.Data;
using FirstApp.Models;

namespace FirstApp.Pages.ObjectTypes
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(int? repositoryId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Forbid();
            }

            // Validate that a valid repositoryId is provided
            if (!repositoryId.HasValue || repositoryId.Value <= 0)
            {
                TempData["ErrorMessage"] = "ObjectTypes must be created from within a Repository. Please select a Repository first.";
                return RedirectToPage("/Designer/Index");
            }

            var now = DateTime.UtcNow;

            // Load the repository for display in breadcrumb
            var repository = _context.Repositories.Find(repositoryId.Value);
            if (repository == null)
            {
                return NotFound();
            }

            // Initialize ObjectType with default values for audit fields
            ObjectType = new ObjectType
            {
                Name = string.Empty,
                Description = string.Empty,
                CreatedById = userId,
                CreatedAt = now,
                UpdatedById = userId,
                UpdatedAt = now,
                RepositoryId = repositoryId.Value,
                CreatedBy = null!,
                UpdatedBy = null!,
                Repository = repository
            };

            // Add system properties to the ObjectType for display
            ObjectType.PropertyTypes = CreateSystemPropertyTypes(userId, now);

            return Page();
        }

        [BindProperty]
        public ObjectType ObjectType { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Forbid();
            }

            var now = DateTime.UtcNow;

            // Set audit fields before validation
            ObjectType.CreatedById = userId;
            ObjectType.CreatedAt = now;
            ObjectType.UpdatedById = userId;
            ObjectType.UpdatedAt = now;

            // Re-create system properties since they're not bound from the form
            ObjectType.PropertyTypes = CreateSystemPropertyTypes(userId, now);

            // Remove validation errors for navigation properties since they're not bound from the form
            ModelState.Remove("ObjectType.CreatedBy");
            ModelState.Remove("ObjectType.UpdatedBy");
            ModelState.Remove("ObjectType.Repository");

            // Validate that a valid RepositoryId is provided
            if (ObjectType.RepositoryId <= 0)
            {
                ModelState.AddModelError(string.Empty, "A valid Repository is required to create an Object Type.");
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ObjectTypes.Add(ObjectType);
            await _context.SaveChangesAsync();

            // If ObjectType was created from a Repository, redirect back to that Repository's details
            if (ObjectType.RepositoryId > 0)
            {
                return RedirectToPage("/Designer/Details", new { id = ObjectType.RepositoryId });
            }

            return RedirectToPage("./Index");
        }


        private static List<PropertyType> CreateSystemPropertyTypes(string userId, DateTime now)
        {
            return new List<PropertyType>
            {
                CreatePropertyType("Name", "The name of the object", PropertyDataType.String, userId, now),
                CreatePropertyType("Description", "A detailed description of     object", PropertyDataType.Memo, userId, now),
                CreatePropertyType("Who Created", "The user who created this object", PropertyDataType.String, userId, now),
                CreatePropertyType("When Created", "The date and time when this object was created", PropertyDataType.DateTime, userId, now),
                CreatePropertyType("Who Updated", "The user who last updated this object", PropertyDataType.String, userId, now),
                CreatePropertyType("When Updated", "The date and time when this object was last updated", PropertyDataType.DateTime, userId, now),
                CreatePropertyType("Category", "The category this object belongs to", PropertyDataType.String, userId, now)
            };
        }

        private static PropertyType CreatePropertyType(string name, string description, PropertyDataType dataType, string userId, DateTime now)
        {
            return new PropertyType
            {
                Name = name,
                Description = description,
                DataType = dataType,
                IsSystemProperty = true,
                ObjectTypeId = 0,
                CreatedById = userId,
                CreatedAt = now,
                UpdatedById = userId,
                UpdatedAt = now
            };
        }
    }
}