using Microsoft.AspNetCore.Identity;

namespace Wyrm.Models
{
    /// <summary>
    /// Represents an integer value recorded for a given <see cref="AssociationPropertyType"/>
    /// on a given <see cref="AssociationInstance"/>. A row's existence means the association
    /// has a value set for that property; there is no null-value row.
    /// </summary>
    public class AssociationPropertyValueInt
    {
        /// <summary>
        /// Gets or sets the unique identifier for this association property value.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the association instance this value belongs to.
        /// </summary>
        public required int AssociationInstanceId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the association instance this value belongs to.
        /// </summary>
        public AssociationInstance? AssociationInstance { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ID of the association property type this value is recorded against.
        /// </summary>
        public required int AssociationPropertyTypeId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the association property type this value is recorded against.
        /// </summary>
        public AssociationPropertyType? AssociationPropertyType { get; set; } = null!;

        /// <summary>
        /// Gets or sets the recorded value.
        /// </summary>
        public required int Value { get; set; }

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
