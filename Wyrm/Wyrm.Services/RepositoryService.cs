using Microsoft.EntityFrameworkCore;
using Wyrm.Data;
using Wyrm.Models;
using Wyrm.ViewModels;

namespace Wyrm.Services
{
    public class RepositoryService(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IRepositoryService
    {
        public async Task<List<Repository>> GetAllWithObjectTypesAsync()
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();
            return await context.Repositories
                .Include(r => r.ObjectTypes)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<List<Repository>> GetAllWithModelsAsync()
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();
            return await context.Repositories
                .Include(r => r.ObjectTypes)
                .Include(r => r.AssociationTypes)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<int?> SaveAsync(RepositoryFormInput input, string userId)
        {
            var now = DateTime.UtcNow;
            await using var context = await dbContextFactory.CreateDbContextAsync();

            if (input.Id.HasValue)
            {
                var repository = await context.Repositories.FindAsync(input.Id.Value);
                if (repository == null)
                {
                    return null;
                }

                repository.Name = input.Name;
                repository.Description = input.Description;
                repository.UpdatedById = userId;
                repository.UpdatedAt = now;
                await context.SaveChangesAsync();
                return repository.Id;
            }

            var newRepository = new Repository
            {
                Name = input.Name,
                Description = input.Description,
                CreatedById = userId,
                CreatedAt = now,
                UpdatedById = userId,
                UpdatedAt = now
            };
            context.Repositories.Add(newRepository);
            await context.SaveChangesAsync();
            return newRepository.Id;
        }

        public async Task<ServiceResult> DeleteAsync(int repositoryId)
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();
            var toDelete = await context.Repositories
                .Include(r => r.ObjectTypes)
                .Include(r => r.AssociationTypes)
                .FirstOrDefaultAsync(r => r.Id == repositoryId);

            if (toDelete == null)
            {
                return ServiceResult.Ok();
            }

            if (toDelete.ObjectTypes.Any() || toDelete.AssociationTypes.Any())
            {
                return ServiceResult.Fail($"Cannot delete '{toDelete.Name}' because it still contains object types or association types.");
            }

            context.Repositories.Remove(toDelete);
            await context.SaveChangesAsync();
            return ServiceResult.Ok();
        }
    }
}
