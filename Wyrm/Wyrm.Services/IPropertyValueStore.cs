using Wyrm.Data;
using Wyrm.Models;

namespace Wyrm.Services
{
    /// <summary>
    /// Dispatches property value reads/writes to the correct one of the four value tables
    /// (PropertyValueStrings/Ints/Numbers/DateTimes) based on a PropertyType's DataType,
    /// so page code doesn't need to repeat that branch.
    /// </summary>
    public interface IPropertyValueStore
    {
        /// <summary>
        /// Loads the existing values for an object instance as a flat PropertyTypeId -> display/edit string map.
        /// A missing key means no value has been recorded for that property yet.
        /// </summary>
        Task<Dictionary<int, string?>> LoadRawValuesAsync(ApplicationDbContext context, int objectInstanceId, IEnumerable<PropertyType> propertyTypes);

        /// <summary>
        /// Stamps the "Who Created"/"When Created"/"Who Updated"/"When Updated" system properties (see
        /// <see cref="SystemPropertyNames"/>) from the instance's own audit fields, rather than from
        /// user input. "Who/When Created" are only stamped on create, so they stay fixed afterwards.
        /// </summary>
        Task SetAuditMirrorValuesAsync(ApplicationDbContext context, ObjectInstance instance, IEnumerable<PropertyType> propertyTypes, string userName, string userId, DateTime now, bool isCreate);

        /// <summary>
        /// Creates, updates, or removes the value row for (instance, propertyType) based on rawValue.
        /// A blank rawValue removes any existing row. The caller must have already validated rawValue
        /// with <see cref="PropertyValueParser.TryValidate"/>.
        /// </summary>
        Task SetValueAsync(ApplicationDbContext context, ObjectInstance instance, PropertyType propertyType, string? rawValue, string userId, DateTime now);
    }
}
