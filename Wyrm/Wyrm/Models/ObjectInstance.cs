using Microsoft.AspNetCore.Identity;

namespace Wyrm.Models
{
    /// <summary>
    /// Represents an instance of data conforming to an <see cref="ObjectType"/>'s schema.
    /// Includes audit tracking for creation and modification and a collection of
    /// property values (split by value type) that hold the instance's actual data.
    /// </summary>
    public class ObjectInstance
    {
        /// <summary>
        /// Gets or sets the unique identifier for this object instance.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the object type this instance conforms to.
        /// </summary>
        public required int ObjectTypeId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the object type this instance conforms to.
        /// </summary>
        public ObjectType? ObjectType { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ID of the user who created this object instance.
        /// </summary>
        public required string CreatedById { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this object instance was created.
        /// </summary>
        public required DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who last updated this object instance.
        /// </summary>
        public required string UpdatedById { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this object instance was last updated.
        /// </summary>
        public required DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the user who created this object instance.
        /// </summary>
        public IdentityUser? CreatedBy { get; set; } = null!;

        /// <summary>
        /// Gets or sets the navigation property to the user who last updated this object instance.
        /// </summary>
        public IdentityUser? UpdatedBy { get; set; } = null!;

        /// <summary>
        /// Gets or sets the string/memo property values recorded against this instance.
        /// </summary>
        public ICollection<PropertyValueString> PropertyValueStrings { get; set; } = new List<PropertyValueString>();

        /// <summary>
        /// Gets or sets the integer property values recorded against this instance.
        /// </summary>
        public ICollection<PropertyValueInt> PropertyValueInts { get; set; } = new List<PropertyValueInt>();

        /// <summary>
        /// Gets or sets the numeric (floating point) property values recorded against this instance.
        /// </summary>
        public ICollection<PropertyValueNumber> PropertyValueNumbers { get; set; } = new List<PropertyValueNumber>();

        /// <summary>
        /// Gets or sets the date/date-time property values recorded against this instance.
        /// </summary>
        public ICollection<PropertyValueDateTime> PropertyValueDateTimes { get; set; } = new List<PropertyValueDateTime>();
    }
}
