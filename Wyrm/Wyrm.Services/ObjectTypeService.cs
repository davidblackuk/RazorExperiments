using Microsoft.EntityFrameworkCore;
using Wyrm.Data;
using Wyrm.Models;
using Wyrm.ViewModels;

namespace Wyrm.Services
{
    public class ObjectTypeService(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IObjectTypeService
    {
        public async Task<ObjectType> GetForDesignerAsync(int objectTypeId)
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();
            return await context.ObjectTypes
                .Include(o => o.Repository)
                .Include(o => o.PropertyTypes.OrderBy(pt => pt.Id))
                .FirstAsync(o => o.Id == objectTypeId);
        }

        public async Task<int?> SaveAsync(ObjectTypeFormInput input, int? repositoryId, string userId)
        {
            var now = DateTime.UtcNow;
            await using var context = await dbContextFactory.CreateDbContextAsync();

            if (input.Id.HasValue)
            {
                var objectType = await context.ObjectTypes.FindAsync(input.Id.Value);
                if (objectType == null)
                {
                    return null;
                }

                objectType.Name = input.Name;
                objectType.PluralName = input.PluralName;
                objectType.Description = input.Description;
                objectType.UpdatedById = userId;
                objectType.UpdatedAt = now;
                await context.SaveChangesAsync();
                return objectType.Id;
            }

            if (!repositoryId.HasValue)
            {
                return null;
            }

            var newObjectType = new ObjectType
            {
                Name = input.Name,
                PluralName = input.PluralName,
                Description = input.Description,
                RepositoryId = repositoryId.Value,
                CreatedById = userId,
                CreatedAt = now,
                UpdatedById = userId,
                UpdatedAt = now,
                PropertyTypes = ObjectTypeSystemProperties.CreateDefaults(userId, now)
            };
            context.ObjectTypes.Add(newObjectType);
            await context.SaveChangesAsync();
            return newObjectType.Id;
        }

        public async Task<ServiceResult> DeleteAsync(int objectTypeId)
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();
            var toDelete = await context.ObjectTypes.FindAsync(objectTypeId);
            if (toDelete == null)
            {
                return ServiceResult.Ok();
            }

            var isReferencedByAssociationType = await context.AssociationTypes
                .AnyAsync(a => a.SourceObjectTypeId == objectTypeId || a.TargetObjectTypeId == objectTypeId);
            if (isReferencedByAssociationType)
            {
                return ServiceResult.Fail($"Cannot delete '{toDelete.Name}' because it is used as the source or target of one or more association types.");
            }

            context.ObjectTypes.Remove(toDelete);
            await context.SaveChangesAsync();
            return ServiceResult.Ok();
        }
    }
}
