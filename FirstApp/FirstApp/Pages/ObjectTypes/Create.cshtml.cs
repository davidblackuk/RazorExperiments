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
                return RedirectToPage("/Repositories/Index");
            }

            // Initialize ObjectType with default values for audit fields
            ObjectType = new ObjectType
            {
                Name = string.Empty,
                Description = string.Empty,
                CreatedById = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedById = userId,
                UpdatedAt = DateTime.UtcNow,
                RepositoryId = repositoryId.Value,
                CreatedBy = null!,
                UpdatedBy = null!,
                Repository = null!
            };

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

            // Add system properties to the ObjectType before saving
            ObjectType.PropertyTypes = new List<PropertyType>
            {
                new PropertyType
                {
                    Name = "Name",
                    Description = "The name of the object",
                    DataType = PropertyDataType.String,
                    IsSystemProperty = true,
                    ObjectTypeId = 0,
                    CreatedById = userId,
                    CreatedAt = now,
                    UpdatedById = userId,
                    UpdatedAt = now,
                },
                new PropertyType
                {
                    Name = "Description",
                    Description = "A detailed description of the object",
                    DataType = PropertyDataType.Memo,
                    IsSystemProperty = true,
                    ObjectTypeId = 0,
                    CreatedById = userId,
                    CreatedAt = now,
                    UpdatedById = userId,
                    UpdatedAt = now,
                },
                new PropertyType
                {
                    Name = "Who Created",
                    Description = "The user who created this object",
                    DataType = PropertyDataType.String,
                    IsSystemProperty = true,
                    ObjectTypeId = 0,
                    CreatedById = userId,
                    CreatedAt = now,
                    UpdatedById = userId,
                    UpdatedAt = now,
                },
                new PropertyType
                {
                    Name = "When Created",
                    Description = "The date and time when this object was created",
                    DataType = PropertyDataType.DateTime,
                    IsSystemProperty = true,
                    ObjectTypeId = 0,
                    CreatedById = userId,
                    CreatedAt = now,
                    UpdatedById = userId,
                    UpdatedAt = now,
                },
                new PropertyType
                {
                    Name = "Who Updated",
                    Description = "The user who last updated this object",
                    DataType = PropertyDataType.String,
                    IsSystemProperty = true,
                    ObjectTypeId = 0,
                    CreatedById = userId,
                    CreatedAt = now,
                    UpdatedById = userId,
                    UpdatedAt = now,
                },
                new PropertyType
                {
                    Name = "When Updated",
                    Description = "The date and time when this object was last updated",
                    DataType = PropertyDataType.DateTime,
                    IsSystemProperty = true,
                    ObjectTypeId = 0,
                    CreatedById = userId,
                    CreatedAt = now,
                    UpdatedById = userId,
                    UpdatedAt = now,
                },
                new PropertyType
                {
                    Name = "Category",
                    Description = "The category this object belongs to",
                    DataType = PropertyDataType.String,
                    IsSystemProperty = true,
                    ObjectTypeId = 0,
                    CreatedById = userId,
                    CreatedAt = now,
                    UpdatedById = userId,
                    UpdatedAt = now,
                }
            };

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
                return RedirectToPage("/Repositories/Details", new { id = ObjectType.RepositoryId });
            }

            return RedirectToPage("./Index");
        }
    }
}
