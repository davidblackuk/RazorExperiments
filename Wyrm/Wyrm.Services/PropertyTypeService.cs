using Microsoft.EntityFrameworkCore;
using Wyrm.Data;
using Wyrm.Models;
using Wyrm.ViewModels;

namespace Wyrm.Services
{
    public class PropertyTypeService(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IPropertyTypeService
    {
        public async Task<PropertyType> GetWithAuditUsersAsync(int propertyTypeId)
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();
            return await context.PropertyTypes
                .Include(p => p.CreatedBy)
                .Include(p => p.UpdatedBy)
                .FirstAsync(p => p.Id == propertyTypeId);
        }

        public async Task<int?> SaveAsync(PropertyTypeFormInput input, int? objectTypeId, string userId)
        {
            var now = DateTime.UtcNow;
            await using var context = await dbContextFactory.CreateDbContextAsync();

            if (input.Id.HasValue)
            {
                var propertyType = await context.PropertyTypes.FindAsync(input.Id.Value);
                if (propertyType == null)
                {
                    return null;
                }

                propertyType.Name = input.Name;
                propertyType.Description = input.Description;
                propertyType.DataType = input.DataType;
                propertyType.UpdatedById = userId;
                propertyType.UpdatedAt = now;
                await context.SaveChangesAsync();
                return propertyType.Id;
            }

            if (!objectTypeId.HasValue)
            {
                return null;
            }

            var newPropertyType = new PropertyType
            {
                Name = input.Name,
                Description = input.Description,
                DataType = input.DataType,
                ObjectTypeId = objectTypeId.Value,
                CreatedById = userId,
                CreatedAt = now,
                UpdatedById = userId,
                UpdatedAt = now
            };
            context.PropertyTypes.Add(newPropertyType);
            await context.SaveChangesAsync();
            return newPropertyType.Id;
        }

        public async Task DeleteAsync(int propertyTypeId)
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();
            var toDelete = await context.PropertyTypes.FindAsync(propertyTypeId);
            if (toDelete != null)
            {
                context.PropertyTypes.Remove(toDelete);
                await context.SaveChangesAsync();
            }
        }
    }
}
