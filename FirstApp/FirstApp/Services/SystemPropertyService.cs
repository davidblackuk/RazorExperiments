using FirstApp.Data;
using FirstApp.Models;

namespace FirstApp.Services
{
    /// <summary>
    /// Service responsible for creating and managing system properties for object types.
    /// </summary>
    public class SystemPropertyService
    {
        private readonly ApplicationDbContext _context;

        public SystemPropertyService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates default system properties for a newly created object type.
        /// </summary>
        /// <param name="objectTypeId">The ID of the object type to add system properties to.</param>
        /// <param name="userId">The ID of the user creating the properties.</param>
        public async Task CreateSystemPropertiesAsync(int objectTypeId, string userId)
        {
            var now = DateTime.UtcNow;

            var systemProperties = new List<PropertyType>
            {
                new PropertyType
                {
                    Name = "Name",
                    Description = "The name of the object",
                    DataType = PropertyDataType.String,
                    IsSystemProperty = true,
                    ObjectTypeId = objectTypeId,
                    CreatedById = userId,
                    CreatedAt = now,
                    UpdatedById = userId,
                    UpdatedAt = now
                },
                new PropertyType
                {
                    Name = "Description",
                    Description = "A detailed description of the object",
                    DataType = PropertyDataType.Memo,
                    IsSystemProperty = true,
                    ObjectTypeId = objectTypeId,
                    CreatedById = userId,
                    CreatedAt = now,
                    UpdatedById = userId,
                    UpdatedAt = now
                },
                new PropertyType
                {
                    Name = "Who Created",
                    Description = "The user who created this object",
                    DataType = PropertyDataType.String,
                    IsSystemProperty = true,
                    ObjectTypeId = objectTypeId,
                    CreatedById = userId,
                    CreatedAt = now,
                    UpdatedById = userId,
                    UpdatedAt = now
                },
                new PropertyType
                {
                    Name = "When Created",
                    Description = "The date and time when this object was created",
                    DataType = PropertyDataType.DateTime,
                    IsSystemProperty = true,
                    ObjectTypeId = objectTypeId,
                    CreatedById = userId,
                    CreatedAt = now,
                    UpdatedById = userId,
                    UpdatedAt = now
                },
                new PropertyType
                {
                    Name = "Who Updated",
                    Description = "The user who last updated this object",
                    DataType = PropertyDataType.String,
                    IsSystemProperty = true,
                    ObjectTypeId = objectTypeId,
                    CreatedById = userId,
                    CreatedAt = now,
                    UpdatedById = userId,
                    UpdatedAt = now
                },
                new PropertyType
                {
                    Name = "When Updated",
                    Description = "The date and time when this object was last updated",
                    DataType = PropertyDataType.DateTime,
                    IsSystemProperty = true,
                    ObjectTypeId = objectTypeId,
                    CreatedById = userId,
                    CreatedAt = now,
                    UpdatedById = userId,
                    UpdatedAt = now
                },
                new PropertyType
                {
                    Name = "Category",
                    Description = "The category this object belongs to",
                    DataType = PropertyDataType.String,
                    IsSystemProperty = true,
                    ObjectTypeId = objectTypeId,
                    CreatedById = userId,
                    CreatedAt = now,
                    UpdatedById = userId,
                    UpdatedAt = now
                }
            };

            _context.PropertyTypes.AddRange(systemProperties);
            await _context.SaveChangesAsync();
        }
    }
}