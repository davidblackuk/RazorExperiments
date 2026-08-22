using Wyrm.Models;
using Wyrm.ViewModels;

namespace Wyrm.Services
{
    public interface IAssociationPropertyTypeService
    {
        /// <summary>
        /// Loads an AssociationPropertyType with its CreatedBy/UpdatedBy navigation properties for the
        /// Designer detail panel.
        /// </summary>
        Task<AssociationPropertyType> GetWithAuditUsersAsync(int associationPropertyTypeId);

        /// <summary>
        /// Creates a new association property type under <paramref name="associationTypeId"/>, or updates an
        /// existing one when <paramref name="input"/>.Id is set. Returns the association property type's Id,
        /// or null if neither a valid create nor a valid update target was supplied.
        /// </summary>
        Task<int?> SaveAsync(AssociationPropertyTypeFormInput input, int? associationTypeId, string userId);

        Task DeleteAsync(int associationPropertyTypeId);
    }
}
