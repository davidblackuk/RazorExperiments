using Microsoft.EntityFrameworkCore;
using Wyrm.Data;
using Wyrm.Models;
using Wyrm.ViewModels;

namespace Wyrm.Services
{
    public class AssociationTypeService(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IAssociationTypeService
    {
        public async Task<AssociationType> GetForDesignerAsync(int associationTypeId)
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();
            return await context.AssociationTypes
                .Include(a => a.Repository)
                .Include(a => a.SourceObjectType)
                .Include(a => a.TargetObjectType)
                .Include(a => a.PropertyTypes.OrderBy(pt => pt.Id))
                .FirstAsync(a => a.Id == associationTypeId);
        }

        public async Task<int?> SaveAsync(AssociationTypeFormInput input, int? repositoryId, string userId)
        {
            var now = DateTime.UtcNow;
            await using var context = await dbContextFactory.CreateDbContextAsync();

            if (input.Id.HasValue)
            {
                var associationType = await context.AssociationTypes.FindAsync(input.Id.Value);
                if (associationType == null)
                {
                    return null;
                }

                associationType.ForwardName = input.ForwardName;
                associationType.ReverseName = input.ReverseName;
                associationType.Description = input.Description;
                associationType.SourceObjectTypeId = input.SourceObjectTypeId;
                associationType.TargetObjectTypeId = input.TargetObjectTypeId;
                associationType.UpdatedById = userId;
                associationType.UpdatedAt = now;
                await context.SaveChangesAsync();
                return associationType.Id;
            }

            if (!repositoryId.HasValue)
            {
                return null;
            }

            var newAssociationType = new AssociationType
            {
                ForwardName = input.ForwardName,
                ReverseName = input.ReverseName,
                Description = input.Description,
                RepositoryId = repositoryId.Value,
                SourceObjectTypeId = input.SourceObjectTypeId,
                TargetObjectTypeId = input.TargetObjectTypeId,
                CreatedById = userId,
                CreatedAt = now,
                UpdatedById = userId,
                UpdatedAt = now
            };
            context.AssociationTypes.Add(newAssociationType);
            await context.SaveChangesAsync();
            return newAssociationType.Id;
        }

        public async Task DeleteAsync(int associationTypeId)
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();
            var toDelete = await context.AssociationTypes.FindAsync(associationTypeId);
            if (toDelete != null)
            {
                context.AssociationTypes.Remove(toDelete);
                await context.SaveChangesAsync();
            }
        }
    }
}
