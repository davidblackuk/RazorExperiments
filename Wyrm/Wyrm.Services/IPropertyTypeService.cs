using Wyrm.Models;
using Wyrm.ViewModels;

namespace Wyrm.Services
{
    public interface IPropertyTypeService
    {
        /// <summary>
        /// Loads a PropertyType with its CreatedBy/UpdatedBy navigation properties for the Designer detail panel.
        /// </summary>
        Task<PropertyType> GetWithAuditUsersAsync(int propertyTypeId);

        /// <summary>
        /// Creates a new property type under <paramref name="objectTypeId"/>, or updates an existing one when
        /// <paramref name="input"/>.Id is set. Returns the property type's Id, or null if neither a valid
        /// create nor a valid update target was supplied.
        /// </summary>
        Task<int?> SaveAsync(PropertyTypeFormInput input, int? objectTypeId, string userId);

        Task DeleteAsync(int propertyTypeId);
    }
}
