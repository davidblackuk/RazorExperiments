using Microsoft.EntityFrameworkCore;
using Wyrm.Data;
using Wyrm.Models;

namespace Wyrm.Services
{
    /// <summary>
    /// Resolves the display label for an ObjectInstance from its "Name" system property value,
    /// falling back to "Instance #{Id}" if that value is absent or blank.
    /// </summary>
    public static class InstanceDisplayHelper
    {
        public static async Task<string> GetDisplayNameAsync(ApplicationDbContext context, ObjectInstance instance)
        {
            var nameValue = await context.PropertyValueStrings
                .Where(v => v.ObjectInstanceId == instance.Id && v.PropertyType!.Name == "Name")
                .Select(v => v.Value)
                .FirstOrDefaultAsync();

            return string.IsNullOrWhiteSpace(nameValue) ? $"Instance #{instance.Id}" : nameValue;
        }

        /// <summary>
        /// Bulk variant for a list of instances that all conform to the same ObjectType, avoiding one query per row.
        /// </summary>
        public static async Task<Dictionary<int, string>> GetDisplayNamesAsync(ApplicationDbContext context, int objectTypeId, IEnumerable<int> objectInstanceIds)
        {
            var instanceIds = objectInstanceIds.ToList();
            var result = instanceIds.ToDictionary(id => id, id => $"Instance #{id}");

            var nameValues = await context.PropertyValueStrings
                .Where(v => v.PropertyType!.ObjectTypeId == objectTypeId
                         && v.PropertyType!.Name == "Name"
                         && instanceIds.Contains(v.ObjectInstanceId))
                .Select(v => new { v.ObjectInstanceId, v.Value })
                .ToListAsync();

            foreach (var nameValue in nameValues)
            {
                if (!string.IsNullOrWhiteSpace(nameValue.Value))
                {
                    result[nameValue.ObjectInstanceId] = nameValue.Value;
                }
            }

            return result;
        }
    }
}
