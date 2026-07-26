using Microsoft.AspNetCore.Identity;

namespace Wyrm.Abstractions
{
    public interface IAuditModifications
    {
        string CreatedById { get; set; }
        DateTime CreatedAt { get; set; }
        string UpdatedById { get; set; }
        DateTime UpdatedAt { get; set; }
        IdentityUser? CreatedBy { get; set; }
        IdentityUser? UpdatedBy { get; set; }
    }
}