using FirstApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FirstApp.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Repository> Repositories { get; set; }
        public DbSet<ObjectType> ObjectTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Repository>()
                .HasOne(r => r.CreatedBy)
                .WithMany()
                .HasForeignKey(r => r.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Repository>()
                .HasOne(r => r.UpdatedBy)
                .WithMany()
                .HasForeignKey(r => r.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ObjectType>()
               .HasOne(r => r.CreatedBy)
               .WithMany()
               .HasForeignKey(r => r.CreatedById)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ObjectType>()
                .HasOne(r => r.UpdatedBy)
                .WithMany()
                .HasForeignKey(r => r.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Repository>()
                .HasOne(r => r.ObjectType)
                .WithMany(o => o.Repositories)
                .HasForeignKey(r => r.ObjectTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
