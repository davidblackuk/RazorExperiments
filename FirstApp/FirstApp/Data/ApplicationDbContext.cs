using FirstApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FirstApp.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Repository> Repositories { get; set; }
        public DbSet<ObjectType> ObjectTypes { get; set; }
        public DbSet<PropertyType> PropertyTypes { get; set; }

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

            modelBuilder.Entity<ObjectType>()
                .HasOne(o => o.Repository)
                .WithMany(r => r.ObjectTypes)
                .HasForeignKey(o => o.RepositoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PropertyType>()
                .ToTable("PropertyTypes");

            modelBuilder.Entity<PropertyType>()
                .HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PropertyType>()
                .HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey(p => p.UpdatedById)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PropertyType>()
                .HasOne(p => p.ObjectType)
                .WithMany(o => o.PropertyTypes)
                .HasForeignKey(p => p.ObjectTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
