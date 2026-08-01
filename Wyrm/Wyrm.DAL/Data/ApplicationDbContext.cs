using Wyrm.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Wyrm.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Repository> Repositories { get; set; }
        public DbSet<ObjectType> ObjectTypes { get; set; }
        public DbSet<PropertyType> PropertyTypes { get; set; }
        public DbSet<ObjectInstance> ObjectInstances { get; set; }
        public DbSet<PropertyValueString> PropertyValueStrings { get; set; }
        public DbSet<PropertyValueInt> PropertyValueInts { get; set; }
        public DbSet<PropertyValueNumber> PropertyValueNumbers { get; set; }
        public DbSet<PropertyValueDateTime> PropertyValueDateTimes { get; set; }

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

            modelBuilder.Entity<ObjectInstance>()
                .HasOne(i => i.CreatedBy)
                .WithMany()
                .HasForeignKey(i => i.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ObjectInstance>()
                .HasOne(i => i.UpdatedBy)
                .WithMany()
                .HasForeignKey(i => i.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ObjectInstance>()
                .HasOne(i => i.ObjectType)
                .WithMany(o => o.ObjectInstances)
                .HasForeignKey(i => i.ObjectTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PropertyValueString>()
                .HasOne(v => v.CreatedBy)
                .WithMany()
                .HasForeignKey(v => v.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PropertyValueString>()
                .HasOne(v => v.UpdatedBy)
                .WithMany()
                .HasForeignKey(v => v.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PropertyValueString>()
                .HasOne(v => v.ObjectInstance)
                .WithMany(i => i.PropertyValueStrings)
                .HasForeignKey(v => v.ObjectInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PropertyValueString>()
                .HasOne(v => v.PropertyType)
                .WithMany()
                .HasForeignKey(v => v.PropertyTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PropertyValueString>()
                .HasIndex(v => new { v.ObjectInstanceId, v.PropertyTypeId })
                .IsUnique();

            modelBuilder.Entity<PropertyValueInt>()
                .HasOne(v => v.CreatedBy)
                .WithMany()
                .HasForeignKey(v => v.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PropertyValueInt>()
                .HasOne(v => v.UpdatedBy)
                .WithMany()
                .HasForeignKey(v => v.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PropertyValueInt>()
                .HasOne(v => v.ObjectInstance)
                .WithMany(i => i.PropertyValueInts)
                .HasForeignKey(v => v.ObjectInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PropertyValueInt>()
                .HasOne(v => v.PropertyType)
                .WithMany()
                .HasForeignKey(v => v.PropertyTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PropertyValueInt>()
                .HasIndex(v => new { v.ObjectInstanceId, v.PropertyTypeId })
                .IsUnique();

            modelBuilder.Entity<PropertyValueNumber>()
                .HasOne(v => v.CreatedBy)
                .WithMany()
                .HasForeignKey(v => v.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PropertyValueNumber>()
                .HasOne(v => v.UpdatedBy)
                .WithMany()
                .HasForeignKey(v => v.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PropertyValueNumber>()
                .HasOne(v => v.ObjectInstance)
                .WithMany(i => i.PropertyValueNumbers)
                .HasForeignKey(v => v.ObjectInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PropertyValueNumber>()
                .HasOne(v => v.PropertyType)
                .WithMany()
                .HasForeignKey(v => v.PropertyTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PropertyValueNumber>()
                .HasIndex(v => new { v.ObjectInstanceId, v.PropertyTypeId })
                .IsUnique();

            modelBuilder.Entity<PropertyValueDateTime>()
                .HasOne(v => v.CreatedBy)
                .WithMany()
                .HasForeignKey(v => v.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PropertyValueDateTime>()
                .HasOne(v => v.UpdatedBy)
                .WithMany()
                .HasForeignKey(v => v.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PropertyValueDateTime>()
                .HasOne(v => v.ObjectInstance)
                .WithMany(i => i.PropertyValueDateTimes)
                .HasForeignKey(v => v.ObjectInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PropertyValueDateTime>()
                .HasOne(v => v.PropertyType)
                .WithMany()
                .HasForeignKey(v => v.PropertyTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PropertyValueDateTime>()
                .HasIndex(v => new { v.ObjectInstanceId, v.PropertyTypeId })
                .IsUnique();
        }
    }
}
