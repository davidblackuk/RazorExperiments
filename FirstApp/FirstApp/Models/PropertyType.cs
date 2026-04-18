using Microsoft.AspNetCore.Identity;

namespace FirstApp.Models
{
    /// <summary>
    /// Represents a property type definition that describes an attribute that can be associated with an object type.
    /// Includes the property's data type, metadata, and audit tracking for creation and modification.
    /// </summary>
    public class PropertyType
    {
        /// <summary>
        /// Gets or sets the unique identifier for this property type.
        /// </summary>
        public int Id { get; set; }


        /// <summary>
        /// Gets or sets the name of the property type.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the property type.
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a system-defined property type.
        /// System properties are typically read-only and cannot be deleted by users.
        /// </summary>
        public bool IsSystemProperty { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who created this property type.
        /// </summary>
        public required string CreatedById { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this property type was created.
        /// </summary>
        public required DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who last updated this property type.
        /// </summary>
        public required string UpdatedById { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this property type was last updated.
        /// </summary>
        public required DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the user who created this property type.
        /// </summary>
        public IdentityUser? CreatedBy { get; set; } = null!;

        /// <summary>
        /// Gets or sets the navigation property to the user who last updated this property type.
        /// </summary>
        public IdentityUser? UpdatedBy { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ID of the object type this property type belongs to.
        /// </summary>
        public required int ObjectTypeId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the object type this property type belongs to.
        /// </summary>
        public ObjectType? ObjectType { get; set; } = null!;

        /// <summary>
        /// Gets or sets the data type of this property (e.g., String, Int, DateTime).
        /// </summary>
        public  PropertyDataType DataType { get; set; }
    }
}