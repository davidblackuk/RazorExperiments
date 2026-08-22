using Microsoft.AspNetCore.Identity;
using Wyrm.Abstractions;

namespace Wyrm.Models
{
    /// <summary>
    /// Represents an actual link between two object instances, conforming to an
    /// <see cref="AssociationType"/>'s schema.
    /// </summary>
    public class AssociationInstance : IAuditModifications
    {
        /// <summary>
        /// Gets or sets the unique identifier for this association instance.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the association type this instance conforms to.
        /// </summary>
        public required int AssociationTypeId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the association type this instance conforms to.
        /// </summary>
        public AssociationType? AssociationType { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ID of the object instance this association originates from.
        /// </summary>
        public required int SourceObjectInstanceId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the object instance this association originates from.
        /// </summary>
        public ObjectInstance? SourceObjectInstance { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ID of the object instance this association points to.
        /// </summary>
        public required int TargetObjectInstanceId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the object instance this association points to.
        /// </summary>
        public ObjectInstance? TargetObjectInstance { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ID of the user who created this association instance.
        /// </summary>
        public required string CreatedById { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this association instance was created.
        /// </summary>
        public required DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who last updated this association instance.
        /// </summary>
        public required string UpdatedById { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this association instance was last updated.
        /// </summary>
        public required DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the user who created this association instance.
        /// </summary>
        public IdentityUser? CreatedBy { get; set; } = null!;

        /// <summary>
        /// Gets or sets the navigation property to the user who last updated this association instance.
        /// </summary>
        public IdentityUser? UpdatedBy { get; set; } = null!;

        /// <summary>
        /// Gets or sets the string/memo property values recorded against this association instance.
        /// </summary>
        public ICollection<AssociationPropertyValueString> AssociationPropertyValueStrings { get; set; } = new List<AssociationPropertyValueString>();

        /// <summary>
        /// Gets or sets the integer property values recorded against this association instance.
        /// </summary>
        public ICollection<AssociationPropertyValueInt> AssociationPropertyValueInts { get; set; } = new List<AssociationPropertyValueInt>();

        /// <summary>
        /// Gets or sets the numeric (floating point) property values recorded against this association instance.
        /// </summary>
        public ICollection<AssociationPropertyValueNumber> AssociationPropertyValueNumbers { get; set; } = new List<AssociationPropertyValueNumber>();

        /// <summary>
        /// Gets or sets the date/date-time property values recorded against this association instance.
        /// </summary>
        public ICollection<AssociationPropertyValueDateTime> AssociationPropertyValueDateTimes { get; set; } = new List<AssociationPropertyValueDateTime>();
    }
}
