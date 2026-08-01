using Wyrm.Models;

namespace Wyrm.Services
{
    /// <summary>
    /// Builds the fixed set of system PropertyTypes every newly-created ObjectType is seeded with:
    /// Name, Description, Category, plus the "Who/When Created/Updated" audit mirrors
    /// (see SystemPropertyNames) whose values are stamped automatically from the instance's own
    /// audit fields rather than user-entered.
    /// </summary>
    public static class ObjectTypeSystemProperties
    {
        public static List<PropertyType> CreateDefaults(string userId, DateTime now)
        {
            return new List<PropertyType>
            {
                Create("Name", "The name of the object", PropertyDataType.String, userId, now),
                Create("Description", "A detailed description of the object", PropertyDataType.Memo, userId, now),
                Create(SystemPropertyNames.WhoCreated, "The user who created this object", PropertyDataType.String, userId, now),
                Create(SystemPropertyNames.WhenCreated, "The date and time when this object was created", PropertyDataType.DateTime, userId, now),
                Create(SystemPropertyNames.WhoUpdated, "The user who last updated this object", PropertyDataType.String, userId, now),
                Create(SystemPropertyNames.WhenUpdated, "The date and time when this object was last updated", PropertyDataType.DateTime, userId, now),
                Create("Category", "The category this object belongs to", PropertyDataType.String, userId, now)
            };
        }

        private static PropertyType Create(string name, string description, PropertyDataType dataType, string userId, DateTime now) =>
            new()
            {
                Name = name,
                Description = description,
                DataType = dataType,
                IsSystemProperty = true,
                ObjectTypeId = 0,
                CreatedById = userId,
                CreatedAt = now,
                UpdatedById = userId,
                UpdatedAt = now
            };
    }
}
