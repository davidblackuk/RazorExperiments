using Microsoft.EntityFrameworkCore;
using Wyrm.Data;
using Wyrm.Models;
using Wyrm.ViewModels;

namespace Wyrm.Services
{
    public class AssociationInstanceService(
        IDbContextFactory<ApplicationDbContext> dbContextFactory,
        IInstanceDisplayHelper instanceDisplayHelper,
        IAssociationPropertyValueStore associationPropertyValueStore) : IAssociationInstanceService
    {
        public async Task<List<AssociatedObjectRow>> GetAssociatedObjectsAsync(int objectInstanceId)
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();

            var asSource = await context.AssociationInstances
                .Include(a => a.AssociationType)
                .Include(a => a.TargetObjectInstance!.ObjectType)
                .Where(a => a.SourceObjectInstanceId == objectInstanceId)
                .ToListAsync();

            var asTarget = await context.AssociationInstances
                .Include(a => a.AssociationType)
                .Include(a => a.SourceObjectInstance!.ObjectType)
                .Where(a => a.TargetObjectInstanceId == objectInstanceId)
                .ToListAsync();

            var rows = new List<AssociatedObjectRow>();

            foreach (var association in asSource)
            {
                var otherName = await instanceDisplayHelper.GetDisplayNameAsync(context, association.TargetObjectInstance!);
                rows.Add(new AssociatedObjectRow(
                    association.Id,
                    association.AssociationType!.ForwardName,
                    association.TargetObjectInstanceId,
                    association.TargetObjectInstance!.ObjectType!.Name,
                    otherName));
            }

            foreach (var association in asTarget)
            {
                var otherName = await instanceDisplayHelper.GetDisplayNameAsync(context, association.SourceObjectInstance!);
                rows.Add(new AssociatedObjectRow(
                    association.Id,
                    association.AssociationType!.ReverseName,
                    association.SourceObjectInstanceId,
                    association.SourceObjectInstance!.ObjectType!.Name,
                    otherName));
            }

            return rows
                .OrderBy(r => r.DirectionLabel)
                .ThenBy(r => r.OtherDisplayName)
                .ToList();
        }

        public async Task<List<EligibleAssociationOption>> GetEligibleAssociationsAsync(int objectInstanceId)
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();

            var instance = await context.ObjectInstances
                .Include(i => i.ObjectType)
                .FirstAsync(i => i.Id == objectInstanceId);

            var objectTypeId = instance.ObjectTypeId;
            var repositoryId = instance.ObjectType!.RepositoryId;

            var associationTypes = await context.AssociationTypes
                .Include(a => a.PropertyTypes.OrderBy(pt => pt.Id))
                .Where(a => a.RepositoryId == repositoryId
                    && ((a.SourceObjectTypeId == null || a.SourceObjectTypeId == objectTypeId)
                     || (a.TargetObjectTypeId == null || a.TargetObjectTypeId == objectTypeId)))
                .ToListAsync();

            var repositoryObjectTypes = await context.ObjectTypes
                .Where(o => o.RepositoryId == repositoryId)
                .Include(o => o.ObjectInstances)
                .ToListAsync();

            var options = new List<EligibleAssociationOption>();

            foreach (var associationType in associationTypes)
            {
                if (associationType.SourceObjectTypeId == null || associationType.SourceObjectTypeId == objectTypeId)
                {
                    var candidates = await BuildCandidatesAsync(context, repositoryObjectTypes, associationType.TargetObjectTypeId, objectInstanceId);
                    options.Add(new EligibleAssociationOption(
                        associationType.Id,
                        associationType.ForwardName,
                        true,
                        candidates,
                        BuildBlankPropertyFields(associationType.PropertyTypes)));
                }

                if (associationType.TargetObjectTypeId == null || associationType.TargetObjectTypeId == objectTypeId)
                {
                    var candidates = await BuildCandidatesAsync(context, repositoryObjectTypes, associationType.SourceObjectTypeId, objectInstanceId);
                    options.Add(new EligibleAssociationOption(
                        associationType.Id,
                        associationType.ReverseName,
                        false,
                        candidates,
                        BuildBlankPropertyFields(associationType.PropertyTypes)));
                }
            }

            return options.OrderBy(o => o.Label).ToList();
        }

        private static List<PropertyFieldInput> BuildBlankPropertyFields(IEnumerable<AssociationPropertyType> propertyTypes) =>
            propertyTypes
                .Select(pt => new PropertyFieldInput { PropertyTypeId = pt.Id, Name = pt.Name, Description = pt.Description, DataType = pt.DataType, RawValue = null })
                .ToList();

        private async Task<List<AssociationCandidateInstance>> BuildCandidatesAsync(ApplicationDbContext context, List<ObjectType> repositoryObjectTypes, int? restrictToObjectTypeId, int excludeInstanceId)
        {
            var relevantTypes = restrictToObjectTypeId.HasValue
                ? repositoryObjectTypes.Where(o => o.Id == restrictToObjectTypeId.Value)
                : repositoryObjectTypes;

            var result = new List<AssociationCandidateInstance>();

            foreach (var objectType in relevantTypes)
            {
                var instanceIds = objectType.ObjectInstances.Select(i => i.Id).Where(id => id != excludeInstanceId).ToList();
                if (instanceIds.Count == 0)
                {
                    continue;
                }

                var names = await instanceDisplayHelper.GetDisplayNamesAsync(context, objectType.Id, instanceIds);
                result.AddRange(instanceIds.Select(id => new AssociationCandidateInstance(id, objectType.Name, names[id])));
            }

            return result.OrderBy(c => c.ObjectTypeName).ThenBy(c => c.DisplayName).ToList();
        }

        public async Task<int> CreateAsync(AssociationInstanceFormInput input, string userId)
        {
            var now = DateTime.UtcNow;
            await using var context = await dbContextFactory.CreateDbContextAsync();

            var associationType = await context.AssociationTypes
                .Include(a => a.PropertyTypes)
                .FirstAsync(a => a.Id == input.AssociationTypeId);

            var newAssociation = new AssociationInstance
            {
                AssociationTypeId = input.AssociationTypeId,
                SourceObjectInstanceId = input.CurrentInstanceIsSource ? input.CurrentInstanceId : input.OtherInstanceId,
                TargetObjectInstanceId = input.CurrentInstanceIsSource ? input.OtherInstanceId : input.CurrentInstanceId,
                CreatedById = userId,
                CreatedAt = now,
                UpdatedById = userId,
                UpdatedAt = now
            };
            context.AssociationInstances.Add(newAssociation);
            await context.SaveChangesAsync();

            foreach (var field in input.PropertyFields)
            {
                var propertyType = associationType.PropertyTypes.First(pt => pt.Id == field.PropertyTypeId);
                await associationPropertyValueStore.SetValueAsync(context, newAssociation, propertyType, field.RawValue, userId, now);
            }
            await context.SaveChangesAsync();

            return newAssociation.Id;
        }

        public async Task DeleteAsync(int associationInstanceId)
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();
            var toDelete = await context.AssociationInstances.FindAsync(associationInstanceId);
            if (toDelete != null)
            {
                context.AssociationInstances.Remove(toDelete);
                await context.SaveChangesAsync();
            }
        }
    }
}
