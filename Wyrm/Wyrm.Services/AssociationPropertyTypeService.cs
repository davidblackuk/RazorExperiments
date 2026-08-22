using Microsoft.EntityFrameworkCore;
using Wyrm.Data;
using Wyrm.Models;
using Wyrm.ViewModels;

namespace Wyrm.Services
{
    public class AssociationPropertyTypeService(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IAssociationPropertyTypeService
    {
        public async Task<AssociationPropertyType> GetWithAuditUsersAsync(int associationPropertyTypeId)
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();
            return await context.AssociationPropertyTypes
                .Include(p => p.CreatedBy)
                .Include(p => p.UpdatedBy)
                .FirstAsync(p => p.Id == associationPropertyTypeId);
        }

        public async Task<int?> SaveAsync(AssociationPropertyTypeFormInput input, int? associationTypeId, string userId)
        {
            var now = DateTime.UtcNow;
            await using var context = await dbContextFactory.CreateDbContextAsync();

            if (input.Id.HasValue)
            {
                var propertyType = await context.AssociationPropertyTypes.FindAsync(input.Id.Value);
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

            if (!associationTypeId.HasValue)
            {
                return null;
            }

            var newPropertyType = new AssociationPropertyType
            {
                Name = input.Name,
                Description = input.Description,
                DataType = input.DataType,
                AssociationTypeId = associationTypeId.Value,
                CreatedById = userId,
                CreatedAt = now,
                UpdatedById = userId,
                UpdatedAt = now
            };
            context.AssociationPropertyTypes.Add(newPropertyType);
            await context.SaveChangesAsync();
            return newPropertyType.Id;
        }

        public async Task DeleteAsync(int associationPropertyTypeId)
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();
            var toDelete = await context.AssociationPropertyTypes.FindAsync(associationPropertyTypeId);
            if (toDelete != null)
            {
                context.AssociationPropertyTypes.Remove(toDelete);
                await context.SaveChangesAsync();
            }
        }
    }
}
