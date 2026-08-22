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
        public DbSet<AssociationType> AssociationTypes { get; set; }
        public DbSet<AssociationPropertyType> AssociationPropertyTypes { get; set; }
        public DbSet<AssociationInstance> AssociationInstances { get; set; }
        public DbSet<AssociationPropertyValueString> AssociationPropertyValueStrings { get; set; }
        public DbSet<AssociationPropertyValueInt> AssociationPropertyValueInts { get; set; }
        public DbSet<AssociationPropertyValueNumber> AssociationPropertyValueNumbers { get; set; }
        public DbSet<AssociationPropertyValueDateTime> AssociationPropertyValueDateTimes { get; set; }

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

            modelBuilder.Entity<AssociationType>()
                .HasOne(a => a.CreatedBy)
                .WithMany()
                .HasForeignKey(a => a.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssociationType>()
                .HasOne(a => a.UpdatedBy)
                .WithMany()
                .HasForeignKey(a => a.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssociationType>()
                .HasOne(a => a.Repository)
                .WithMany(r => r.AssociationTypes)
                .HasForeignKey(a => a.RepositoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Two separate FKs from AssociationType to ObjectTypes - each needs its own
            // explicit HasOne/HasForeignKey/WithMany() call with no inverse navigation,
            // or EF Core can't tell the two relationships apart.
            modelBuilder.Entity<AssociationType>()
                .HasOne(a => a.SourceObjectType)
                .WithMany()
                .HasForeignKey(a => a.SourceObjectTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssociationType>()
                .HasOne(a => a.TargetObjectType)
                .WithMany()
                .HasForeignKey(a => a.TargetObjectTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssociationPropertyType>()
                .HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AssociationPropertyType>()
                .HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey(p => p.UpdatedById)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AssociationPropertyType>()
                .HasOne(p => p.AssociationType)
                .WithMany(a => a.PropertyTypes)
                .HasForeignKey(p => p.AssociationTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssociationInstance>()
                .HasOne(i => i.CreatedBy)
                .WithMany()
                .HasForeignKey(i => i.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssociationInstance>()
                .HasOne(i => i.UpdatedBy)
                .WithMany()
                .HasForeignKey(i => i.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssociationInstance>()
                .HasOne(i => i.AssociationType)
                .WithMany(a => a.AssociationInstances)
                .HasForeignKey(i => i.AssociationTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cascade (not Restrict, unlike AssociationType.SourceObjectType/TargetObjectType) - deleting an
            // ObjectInstance should take its associations with it, the same way deleting an instance already
            // cascades its PropertyValues, rather than blocking the delete.
            modelBuilder.Entity<AssociationInstance>()
                .HasOne(i => i.SourceObjectInstance)
                .WithMany()
                .HasForeignKey(i => i.SourceObjectInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssociationInstance>()
                .HasOne(i => i.TargetObjectInstance)
                .WithMany()
                .HasForeignKey(i => i.TargetObjectInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssociationPropertyValueString>()
                .HasOne(v => v.CreatedBy)
                .WithMany()
                .HasForeignKey(v => v.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssociationPropertyValueString>()
                .HasOne(v => v.UpdatedBy)
                .WithMany()
                .HasForeignKey(v => v.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssociationPropertyValueString>()
                .HasOne(v => v.AssociationInstance)
                .WithMany(i => i.AssociationPropertyValueStrings)
                .HasForeignKey(v => v.AssociationInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssociationPropertyValueString>()
                .HasOne(v => v.AssociationPropertyType)
                .WithMany()
                .HasForeignKey(v => v.AssociationPropertyTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssociationPropertyValueString>()
                .HasIndex(v => new { v.AssociationInstanceId, v.AssociationPropertyTypeId })
                .IsUnique();

            modelBuilder.Entity<AssociationPropertyValueInt>()
                .HasOne(v => v.CreatedBy)
                .WithMany()
                .HasForeignKey(v => v.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssociationPropertyValueInt>()
                .HasOne(v => v.UpdatedBy)
                .WithMany()
                .HasForeignKey(v => v.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssociationPropertyValueInt>()
                .HasOne(v => v.AssociationInstance)
                .WithMany(i => i.AssociationPropertyValueInts)
                .HasForeignKey(v => v.AssociationInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssociationPropertyValueInt>()
                .HasOne(v => v.AssociationPropertyType)
                .WithMany()
                .HasForeignKey(v => v.AssociationPropertyTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssociationPropertyValueInt>()
                .HasIndex(v => new { v.AssociationInstanceId, v.AssociationPropertyTypeId })
                .IsUnique();

            modelBuilder.Entity<AssociationPropertyValueNumber>()
                .HasOne(v => v.CreatedBy)
                .WithMany()
                .HasForeignKey(v => v.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssociationPropertyValueNumber>()
                .HasOne(v => v.UpdatedBy)
                .WithMany()
                .HasForeignKey(v => v.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssociationPropertyValueNumber>()
                .HasOne(v => v.AssociationInstance)
                .WithMany(i => i.AssociationPropertyValueNumbers)
                .HasForeignKey(v => v.AssociationInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssociationPropertyValueNumber>()
                .HasOne(v => v.AssociationPropertyType)
                .WithMany()
                .HasForeignKey(v => v.AssociationPropertyTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssociationPropertyValueNumber>()
                .HasIndex(v => new { v.AssociationInstanceId, v.AssociationPropertyTypeId })
                .IsUnique();

            modelBuilder.Entity<AssociationPropertyValueDateTime>()
                .HasOne(v => v.CreatedBy)
                .WithMany()
                .HasForeignKey(v => v.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssociationPropertyValueDateTime>()
                .HasOne(v => v.UpdatedBy)
                .WithMany()
                .HasForeignKey(v => v.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssociationPropertyValueDateTime>()
                .HasOne(v => v.AssociationInstance)
                .WithMany(i => i.AssociationPropertyValueDateTimes)
                .HasForeignKey(v => v.AssociationInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssociationPropertyValueDateTime>()
                .HasOne(v => v.AssociationPropertyType)
                .WithMany()
                .HasForeignKey(v => v.AssociationPropertyTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssociationPropertyValueDateTime>()
                .HasIndex(v => new { v.AssociationInstanceId, v.AssociationPropertyTypeId })
                .IsUnique();
        }
    }
}
