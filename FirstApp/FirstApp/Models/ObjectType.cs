using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstApp.Models
{
    public class ObjectType
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

        public required int RepositoryId { get; set; }

        public Repository? Repository { get; set; } = null!;
    }
}
        