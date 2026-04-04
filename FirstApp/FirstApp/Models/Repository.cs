using Microsoft.AspNetCore.Identity;

namespace FirstApp.Models
{
    public class Repository
    {
        public int Id { get; set; }


        public required string Name { get; set; }

        public required string Description { get; set; }

        public required string CreatedById { get; set; }

        public required DateTime CreatedAt { get; set; }

        public required string UpdatedById { get; set; }

        public required DateTime UpdatedAt { get; set; }

        public required IdentityUser CreatedBy { get; set; }

        public required IdentityUser UpdatedBy { get; set; }
        
    }
}
        