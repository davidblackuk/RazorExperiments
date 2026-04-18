using Microsoft.AspNetCore.Identity;
using System.Runtime.InteropServices;

namespace FirstApp.Models
{
    /// <summary>
    /// Represents a repository entity that contains object types and their associated metadata.
    /// </summary>
    public class Repository
    {
        /// <summary>
        /// Gets or sets the unique identifier for the repository.
        /// </summary>
        public int Id { get; set; }


        /// <summary>
        /// Gets or sets the name of the repository.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the repository.
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who created the repository.
        /// </summary>
        public required string CreatedById { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the repository was created.
        /// </summary>
        public required DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who last updated the repository.
        /// </summary>
        public required string UpdatedById { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the repository was last updated.
        /// </summary>
        public required DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the user who created the repository.
        /// Navigation property to <see cref="IdentityUser"/>.
        /// </summary>
        public IdentityUser? CreatedBy { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user who last updated the repository.
        /// Navigation property to <see cref="IdentityUser"/>.
        /// </summary>
        public IdentityUser? UpdatedBy { get; set; } = null!;

        /// <summary>
        /// Gets or sets the collection of object types associated with this repository.
        /// </summary>
        public ICollection<ObjectType> ObjectTypes { get; set; } = new List<ObjectType>();
    }
}