using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstApp.Models
{
    /// <summary>
    /// Represents a type of object within a repository that defines the structure and metadata for objects.
    /// Includes audit tracking for creation and modification and most importantly , a collection of associated 
    /// property types that define the attributes of objects of this type.
    /// </summary>
    public class ObjectType
    {
        /// <summary>
        /// Gets or sets the unique identifier for this object type.
        /// </summary>
        public int Id { get; set; }


        /// <summary>
        /// Gets or sets the name of the object type.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the object type.
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who created this object type.
        /// </summary>
        public required string CreatedById { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this object type was created.
        /// </summary>
        public required DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who last updated this object type.
        /// </summary>
        public required string UpdatedById { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this object type was last updated.
        /// </summary>
        public required DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the user who created this object type.
        /// </summary>
        public IdentityUser? CreatedBy { get; set; } = null!;

        /// <summary>
        /// Gets or sets the navigation property to the user who last updated this object type.
        /// </summary>
        public IdentityUser? UpdatedBy { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ID of the repository that contains this object type.
        /// </summary>
        public required int RepositoryId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the repository that contains this object type.
        /// </summary>
        public Repository? Repository { get; set; } = null!;

        /// <summary>
        /// Gets or sets the collection of property types associated with this object type.
        /// </summary>
        public ICollection<PropertyType> PropertyTypes { get; set; } = new List<PropertyType>();
    }
}