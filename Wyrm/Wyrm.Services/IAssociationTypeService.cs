using Wyrm.Models;
using Wyrm.ViewModels;

namespace Wyrm.Services
{
    public interface IAssociationTypeService
    {
        /// <summary>
        /// Loads an AssociationType with its Repository, Source/Target ObjectTypes, and PropertyTypes
        /// (ordered by Id) for the Designer.
        /// </summary>
        Task<AssociationType> GetForDesignerAsync(int associationTypeId);

        /// <summary>
        /// Creates a new association type under <paramref name="repositoryId"/>, or updates an existing one
        /// when <paramref name="input"/>.Id is set. Returns the association type's Id, or null if neither a
        /// valid create nor a valid update target was supplied.
        /// </summary>
        Task<int?> SaveAsync(AssociationTypeFormInput input, int? repositoryId, string userId);

        Task DeleteAsync(int associationTypeId);
    }
}
