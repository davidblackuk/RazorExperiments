using Microsoft.EntityFrameworkCore;
using Wyrm.Data;
using Wyrm.Models;
using Wyrm.ViewModels;

namespace Wyrm.Services
{
    public class ObjectInstanceService(
        IDbContextFactory<ApplicationDbContext> dbContextFactory,
        IInstanceDisplayHelper instanceDisplayHelper,
        IPropertyValueStore propertyValueStore) : IObjectInstanceService
    {
        private static List<PropertyType> EditableFields(IEnumerable<PropertyType> propertyTypes) =>
            propertyTypes.Where(pt => !SystemPropertyNames.IsAuditMirror(pt.Name)).ToList();

        public async Task<ObjectTypeExplorerView> GetRowsForObjectTypeAsync(int objectTypeId)
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();
            var loaded = await context.ObjectTypes
                .Include(o => o.Repository)
                .Include(o => o.PropertyTypes.OrderBy(pt => pt.Id))
                .Include(o => o.ObjectInstances).ThenInclude(i => i.CreatedBy)
                .Include(o => o.ObjectInstances).ThenInclude(i => i.UpdatedBy)
                .FirstAsync(o => o.Id == objectTypeId);

            var displayNames = await instanceDisplayHelper.GetDisplayNamesAsync(context, loaded.Id, loaded.ObjectInstances.Select(i => i.Id));

            var rows = loaded.ObjectInstances
                .Select(i => new ExplorerInstanceRow
                {
                    Id = i.Id,
                    DisplayName = displayNames[i.Id],
                    CreatedByUserName = i.CreatedBy?.UserName,
                    UpdatedByUserName = i.UpdatedBy?.UserName
                })
                .OrderBy(r => r.DisplayName)
                .ToList();

            return new ObjectTypeExplorerView(loaded, rows);
        }

        public async Task<ExplorerInstanceDetailViewModel> GetDetailAsync(int instanceId)
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();
            var instance = await context.ObjectInstances
                .Include(i => i.ObjectType!.Repository)
                .Include(i => i.ObjectType!.PropertyTypes.OrderBy(pt => pt.Id))
                .Include(i => i.CreatedBy)
                .Include(i => i.UpdatedBy)
                .FirstAsync(i => i.Id == instanceId);

            var values = await propertyValueStore.LoadRawValuesAsync(context, instance.Id, instance.ObjectType!.PropertyTypes);
            var displayName = await instanceDisplayHelper.GetDisplayNameAsync(context, instance);

            return new ExplorerInstanceDetailViewModel
            {
                ObjectInstance = instance,
                DisplayName = displayName,
                Values = values
            };
        }

        public async Task<InstanceEditFormView> GetEditFormFieldsAsync(int instanceId)
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();
            var instance = await context.ObjectInstances
                .Include(i => i.ObjectType!.PropertyTypes.OrderBy(pt => pt.Id))
                .FirstAsync(i => i.Id == instanceId);

            var editable = EditableFields(instance.ObjectType!.PropertyTypes);
            var existingValues = await propertyValueStore.LoadRawValuesAsync(context, instance.Id, editable);
            var displayName = await instanceDisplayHelper.GetDisplayNameAsync(context, instance);

            var fields = editable
                .Select(pt => new PropertyFieldInput
                {
                    PropertyTypeId = pt.Id,
                    Name = pt.Name,
                    Description = pt.Description,
                    DataType = pt.DataType,
                    RawValue = existingValues.GetValueOrDefault(pt.Id)
                })
                .ToList();

            return new InstanceEditFormView(displayName, fields);
        }

        public async Task<int> SaveAsync(int? instanceId, int objectTypeId, IReadOnlyList<PropertyFieldInput> fields, string userId)
        {
            var now = DateTime.UtcNow;
            await using var context = await dbContextFactory.CreateDbContextAsync();

            if (instanceId.HasValue)
            {
                var instance = await context.ObjectInstances
                    .Include(i => i.ObjectType!.PropertyTypes.OrderBy(pt => pt.Id))
                    .FirstAsync(i => i.Id == instanceId.Value);

                instance.UpdatedById = userId;
                instance.UpdatedAt = now;

                await SavePropertyValuesAsync(context, instance, instance.ObjectType!.PropertyTypes, fields, userId, now, isCreate: false);
                return instance.Id;
            }

            var objectType = await context.ObjectTypes
                .Include(o => o.PropertyTypes.OrderBy(pt => pt.Id))
                .FirstAsync(o => o.Id == objectTypeId);

            var newInstance = new ObjectInstance
            {
                ObjectTypeId = objectType.Id,
                CreatedById = userId,
                CreatedAt = now,
                UpdatedById = userId,
                UpdatedAt = now
            };
            context.ObjectInstances.Add(newInstance);
            await context.SaveChangesAsync();

            await SavePropertyValuesAsync(context, newInstance, objectType.PropertyTypes, fields, userId, now, isCreate: true);
            return newInstance.Id;
        }

        private async Task SavePropertyValuesAsync(ApplicationDbContext context, ObjectInstance instance, IEnumerable<PropertyType> propertyTypes, IReadOnlyList<PropertyFieldInput> fields, string userId, DateTime now, bool isCreate)
        {
            var propertyTypeList = propertyTypes.ToList();
            var editable = EditableFields(propertyTypeList);

            foreach (var field in fields)
            {
                var propertyType = editable.First(pt => pt.Id == field.PropertyTypeId);
                await propertyValueStore.SetValueAsync(context, instance, propertyType, field.RawValue, userId, now);
            }

            var user = await context.Users.FindAsync(userId);
            await propertyValueStore.SetAuditMirrorValuesAsync(context, instance, propertyTypeList, user?.UserName ?? userId, userId, now, isCreate);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int instanceId)
        {
            await using var context = await dbContextFactory.CreateDbContextAsync();
            var instance = await context.ObjectInstances.FindAsync(instanceId);
            if (instance != null)
            {
                context.ObjectInstances.Remove(instance);
                await context.SaveChangesAsync();
            }
        }
    }
}
