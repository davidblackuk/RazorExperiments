using Wyrm.Models;
using Wyrm.ViewModels;

namespace Wyrm.Services
{
    public interface IObjectTypeService
    {
        /// <summary>
        /// Loads an ObjectType with its Repository and PropertyTypes (ordered by Id) for the Designer.
        /// </summary>
        Task<ObjectType> GetForDesignerAsync(int objectTypeId);

        /// <summary>
        /// Creates a new object type under <paramref name="repositoryId"/> (seeded with the default system
        /// property types), or updates an existing one when <paramref name="input"/>.Id is set. Returns the
        /// object type's Id, or null if neither a valid create nor a valid update target was supplied.
        /// </summary>
        Task<int?> SaveAsync(ObjectTypeFormInput input, int? repositoryId, string userId);

        /// <summary>
        /// Deletes an object type, refusing (via a failed result) while it is used as the source or target
        /// of an association type.
        /// </summary>
        Task<ServiceResult> DeleteAsync(int objectTypeId);
    }
}
