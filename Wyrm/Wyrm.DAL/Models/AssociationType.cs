using Microsoft.AspNetCore.Identity;
using Wyrm.Abstractions;

namespace Wyrm.Models
{
    /// <summary>
    /// Represents a directional relationship type between two object types within a repository.
    /// Includes audit tracking for creation and modification and a collection of associated
    /// property types that define the attributes of associations of this type.
    /// </summary>
    public class AssociationType : IAuditModifications
    {
        /// <summary>
        /// Gets or sets the unique identifier for this association type.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the association read from source to target (e.g. "Parent of").
        /// </summary>
        public required string ForwardName { get; set; }

        /// <summary>
        /// Gets or sets the name of the association read from target to source (e.g. "Child of").
        /// </summary>
        public required string ReverseName { get; set; }

        /// <summary>
        /// Gets or sets the description of the association type.
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who created this association type.
        /// </summary>
        public required string CreatedById { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this association type was created.
        /// </summary>
        public required DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who last updated this association type.
        /// </summary>
        public required string UpdatedById { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this association type was last updated.
        /// </summary>
        public required DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the user who created this association type.
        /// </summary>
        public IdentityUser? CreatedBy { get; set; } = null!;

        /// <summary>
        /// Gets or sets the navigation property to the user who last updated this association type.
        /// </summary>
        public IdentityUser? UpdatedBy { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ID of the repository that contains this association type.
        /// </summary>
        public required int RepositoryId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the repository that contains this association type.
        /// </summary>
        public Repository? Repository { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ID of the object type this association originates from, or null if the
        /// source may be any object type.
        /// </summary>
        public int? SourceObjectTypeId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the object type this association originates from.
        /// Null means the source may be any object type.
        /// </summary>
        public ObjectType? SourceObjectType { get; set; }

        /// <summary>
        /// Gets or sets the ID of the object type this association points to, or null if the target
        /// may be any object type.
        /// </summary>
        public int? TargetObjectTypeId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the object type this association points to.
        /// Null means the target may be any object type.
        /// </summary>
        public ObjectType? TargetObjectType { get; set; }

        /// <summary>
        /// Gets or sets the collection of property types associated with this association type.
        /// </summary>
        public ICollection<AssociationPropertyType> PropertyTypes { get; set; } = new List<AssociationPropertyType>();

        /// <summary>
        /// Gets or sets the collection of association instances that conform to this association type.
        /// </summary>
        public ICollection<AssociationInstance> AssociationInstances { get; set; } = new List<AssociationInstance>();
    }
}
