using Wyrm.Data;
using Wyrm.Models;

namespace Wyrm.Services
{
    /// <summary>
    /// Dispatches association property value reads/writes to the correct one of the four value tables
    /// (AssociationPropertyValueStrings/Ints/Numbers/DateTimes) based on an AssociationPropertyType's
    /// DataType, so page code doesn't need to repeat that branch. Mirrors <see cref="IPropertyValueStore"/>
    /// one level down (AssociationInstance/AssociationPropertyType instead of ObjectInstance/PropertyType);
    /// there's no audit-mirror equivalent since AssociationType has no auto-seeded system properties.
    /// </summary>
    public interface IAssociationPropertyValueStore
    {
        /// <summary>
        /// Loads the existing values for an association instance as a flat AssociationPropertyTypeId ->
        /// display/edit string map. A missing key means no value has been recorded for that property yet.
        /// </summary>
        Task<Dictionary<int, string?>> LoadRawValuesAsync(ApplicationDbContext context, int associationInstanceId, IEnumerable<AssociationPropertyType> associationPropertyTypes);

        /// <summary>
        /// Creates, updates, or removes the value row for (associationInstance, associationPropertyType)
        /// based on rawValue. A blank rawValue removes any existing row. The caller must have already
        /// validated rawValue with <see cref="PropertyValueParser.TryValidate"/>.
        /// </summary>
        Task SetValueAsync(ApplicationDbContext context, AssociationInstance associationInstance, AssociationPropertyType associationPropertyType, string? rawValue, string userId, DateTime now);
    }
}
