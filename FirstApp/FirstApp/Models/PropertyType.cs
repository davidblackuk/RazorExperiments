using Microsoft.AspNetCore.Identity;

namespace FirstApp.Models
{
    public class PropertyType
    {
        public int Id { get; set; }


        public required string Name { get; set; }

        public required string Description { get; set; }

        public required string CreatedById { get; set; }

        public required DateTime CreatedAt { get; set; }

        public required string UpdatedById { get; set; }

        public required DateTime UpdatedAt { get; set; }

        public IdentityUser? CreatedBy { get; set; } = null!;

        public IdentityUser? UpdatedBy { get; set; } = null!;

        public required int ObjectTypeId { get; set; }

        public ObjectType? ObjectType { get; set; } = null!;
    }
}
        