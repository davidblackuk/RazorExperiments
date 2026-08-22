using Wyrm.Models;
using Wyrm.ViewModels;

namespace Wyrm.Services
{
    public interface IRepositoryService
    {
        Task<List<Repository>> GetAllWithObjectTypesAsync();

        /// <summary>
        /// Loads all repositories with both their ObjectTypes and AssociationTypes, for the Designer's
        /// schema tree.
        /// </summary>
        Task<List<Repository>> GetAllWithModelsAsync();

        /// <summary>
        /// Creates a new repository, or updates an existing one when <paramref name="input"/>.Id is set.
        /// Returns the repository's Id, or null if an update target no longer exists.
        /// </summary>
        Task<int?> SaveAsync(RepositoryFormInput input, string userId);

        /// <summary>
        /// Deletes a repository, refusing (via a failed result) while it still has object types or
        /// association types.
        /// </summary>
        Task<ServiceResult> DeleteAsync(int repositoryId);
    }
}
