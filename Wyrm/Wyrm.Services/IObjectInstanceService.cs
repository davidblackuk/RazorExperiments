using Wyrm.Models;
using Wyrm.ViewModels;

namespace Wyrm.Services
{
    /// <summary>
    /// The ObjectType (with its Repository/PropertyTypes) plus the display-ready instance rows for
    /// the Explorer grid, loaded together since the grid needs both the type's own fields and its instances.
    /// </summary>
    public record ObjectTypeExplorerView(ObjectType ObjectType, List<ExplorerInstanceRow> Rows);

    /// <summary>
    /// The prefilled field values for an instance create/edit form, plus the instance's resolved display name
    /// (used for the modal title on edit).
    /// </summary>
    public record InstanceEditFormView(string DisplayName, List<PropertyFieldInput> Fields);

    public interface IObjectInstanceService
    {
        Task<ObjectTypeExplorerView> GetRowsForObjectTypeAsync(int objectTypeId);

        Task<ExplorerInstanceDetailViewModel> GetDetailAsync(int instanceId);

        Task<InstanceEditFormView> GetEditFormFieldsAsync(int instanceId);

        /// <summary>
        /// Creates a new instance under <paramref name="objectTypeId"/> when <paramref name="instanceId"/> is
        /// null, or updates the existing instance otherwise - writing all of <paramref name="fields"/>'
        /// property values and the audit-mirror system properties in one unit of work. Returns the instance's Id.
        /// </summary>
        Task<int> SaveAsync(int? instanceId, int objectTypeId, IReadOnlyList<PropertyFieldInput> fields, string userId);

        Task DeleteAsync(int instanceId);
    }
}
