using Wyrm.Data;
using Wyrm.Models;

namespace Wyrm.Services
{
    /// <summary>
    /// Resolves the display label for an ObjectInstance from its "Name" system property value,
    /// falling back to "Instance #{Id}" if that value is absent or blank.
    /// </summary>
    public interface IInstanceDisplayHelper
    {
        Task<string> GetDisplayNameAsync(ApplicationDbContext context, ObjectInstance instance);

        /// <summary>
        /// Bulk variant for a list of instances that all conform to the same ObjectType, avoiding one query per row.
        /// </summary>
        Task<Dictionary<int, string>> GetDisplayNamesAsync(ApplicationDbContext context, int objectTypeId, IEnumerable<int> objectInstanceIds);
    }
}
