using Microsoft.AspNetCore.Identity;

namespace Wyrm.Models
{
    /// <summary>
    /// Represents a floating-point numeric value recorded for a given <see cref="PropertyType"/>
    /// on a given <see cref="ObjectInstance"/>. A row's existence means the instance
    /// has a value set for that property; there is no null-value row.
    /// </summary>
    public class PropertyValueNumber
    {
        /// <summary>
        /// Gets or sets the unique identifier for this property value.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the object instance this value belongs to.
        /// </summary>
        public required int ObjectInstanceId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the object instance this value belongs to.
        /// </summary>
        public ObjectInstance? ObjectInstance { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ID of the property type this value is recorded against.
        /// </summary>
        public required int PropertyTypeId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the property type this value is recorded against.
        /// </summary>
        public PropertyType? PropertyType { get; set; } = null!;

        /// <summary>
        /// Gets or sets the recorded value.
        /// </summary>
        public required double Value { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who created this property value.
        /// </summary>
        public required string CreatedById { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this property value was created.
        /// </summary>
        public required DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who last updated this property value.
        /// </summary>
        public required string UpdatedById { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this property value was last updated.
        /// </summary>
        public required DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the user who created this property value.
        /// </summary>
        public IdentityUser? CreatedBy { get; set; } = null!;

        /// <summary>
        /// Gets or sets the navigation property to the user who last updated this property value.
        /// </summary>
        public IdentityUser? UpdatedBy { get; set; } = null!;
    }
}
