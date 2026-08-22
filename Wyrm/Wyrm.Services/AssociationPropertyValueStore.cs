using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Wyrm.Data;
using Wyrm.Models;

namespace Wyrm.Services
{
    public class AssociationPropertyValueStore : IAssociationPropertyValueStore
    {
        public async Task<Dictionary<int, string?>> LoadRawValuesAsync(ApplicationDbContext context, int associationInstanceId, IEnumerable<AssociationPropertyType> associationPropertyTypes)
        {
            var propertyTypeList = associationPropertyTypes.ToList();
            var result = new Dictionary<int, string?>();

            var stringIds = propertyTypeList.Where(pt => pt.DataType is PropertyDataType.String or PropertyDataType.Memo).Select(pt => pt.Id).ToList();
            var intIds = propertyTypeList.Where(pt => pt.DataType == PropertyDataType.Int).Select(pt => pt.Id).ToList();
            var numberIds = propertyTypeList.Where(pt => pt.DataType == PropertyDataType.Number).Select(pt => pt.Id).ToList();
            var dateTimeIds = propertyTypeList.Where(pt => pt.DataType is PropertyDataType.Date or PropertyDataType.DateTime).Select(pt => pt.Id).ToList();

            foreach (var row in await context.AssociationPropertyValueStrings.Where(v => v.AssociationInstanceId == associationInstanceId && stringIds.Contains(v.AssociationPropertyTypeId)).ToListAsync())
            {
                result[row.AssociationPropertyTypeId] = row.Value;
            }

            foreach (var row in await context.AssociationPropertyValueInts.Where(v => v.AssociationInstanceId == associationInstanceId && intIds.Contains(v.AssociationPropertyTypeId)).ToListAsync())
            {
                result[row.AssociationPropertyTypeId] = row.Value.ToString(CultureInfo.InvariantCulture);
            }

            foreach (var row in await context.AssociationPropertyValueNumbers.Where(v => v.AssociationInstanceId == associationInstanceId && numberIds.Contains(v.AssociationPropertyTypeId)).ToListAsync())
            {
                result[row.AssociationPropertyTypeId] = row.Value.ToString(CultureInfo.InvariantCulture);
            }

            var dataTypeByPropertyTypeId = propertyTypeList.ToDictionary(pt => pt.Id, pt => pt.DataType);
            foreach (var row in await context.AssociationPropertyValueDateTimes.Where(v => v.AssociationInstanceId == associationInstanceId && dateTimeIds.Contains(v.AssociationPropertyTypeId)).ToListAsync())
            {
                var format = dataTypeByPropertyTypeId[row.AssociationPropertyTypeId] == PropertyDataType.Date ? "yyyy-MM-dd" : "yyyy-MM-ddTHH:mm";
                result[row.AssociationPropertyTypeId] = row.Value.ToString(format, CultureInfo.InvariantCulture);
            }

            return result;
        }

        public async Task SetValueAsync(ApplicationDbContext context, AssociationInstance associationInstance, AssociationPropertyType associationPropertyType, string? rawValue, string userId, DateTime now)
        {
            switch (associationPropertyType.DataType)
            {
                case PropertyDataType.String:
                case PropertyDataType.Memo:
                    await SetStringValueAsync(context, associationInstance, associationPropertyType, rawValue, userId, now);
                    break;
                case PropertyDataType.Int:
                    await SetIntValueAsync(context, associationInstance, associationPropertyType, rawValue, userId, now);
                    break;
                case PropertyDataType.Number:
                    await SetNumberValueAsync(context, associationInstance, associationPropertyType, rawValue, userId, now);
                    break;
                case PropertyDataType.Date:
                case PropertyDataType.DateTime:
                    await SetDateTimeValueAsync(context, associationInstance, associationPropertyType, rawValue, userId, now);
                    break;
            }
        }

        private static async Task SetStringValueAsync(ApplicationDbContext context, AssociationInstance associationInstance, AssociationPropertyType propertyType, string? rawValue, string userId, DateTime now)
        {
            var existing = await context.AssociationPropertyValueStrings.FirstOrDefaultAsync(v => v.AssociationInstanceId == associationInstance.Id && v.AssociationPropertyTypeId == propertyType.Id);

            if (string.IsNullOrEmpty(rawValue))
            {
                if (existing != null)
                {
                    context.AssociationPropertyValueStrings.Remove(existing);
                }
                return;
            }

            if (existing != null)
            {
                existing.Value = rawValue;
                existing.UpdatedById = userId;
                existing.UpdatedAt = now;
            }
            else
            {
                context.AssociationPropertyValueStrings.Add(new AssociationPropertyValueString
                {
                    AssociationInstanceId = associationInstance.Id,
                    AssociationPropertyTypeId = propertyType.Id,
                    Value = rawValue,
                    CreatedById = userId,
                    CreatedAt = now,
                    UpdatedById = userId,
                    UpdatedAt = now
                });
            }
        }

        private static async Task SetIntValueAsync(ApplicationDbContext context, AssociationInstance associationInstance, AssociationPropertyType propertyType, string? rawValue, string userId, DateTime now)
        {
            var existing = await context.AssociationPropertyValueInts.FirstOrDefaultAsync(v => v.AssociationInstanceId == associationInstance.Id && v.AssociationPropertyTypeId == propertyType.Id);

            if (string.IsNullOrWhiteSpace(rawValue))
            {
                if (existing != null)
                {
                    context.AssociationPropertyValueInts.Remove(existing);
                }
                return;
            }

            if (!PropertyValueParser.TryParseInt(rawValue, out var value, out _))
            {
                return;
            }

            if (existing != null)
            {
                existing.Value = value;
                existing.UpdatedById = userId;
                existing.UpdatedAt = now;
            }
            else
            {
                context.AssociationPropertyValueInts.Add(new AssociationPropertyValueInt
                {
                    AssociationInstanceId = associationInstance.Id,
                    AssociationPropertyTypeId = propertyType.Id,
                    Value = value,
                    CreatedById = userId,
                    CreatedAt = now,
                    UpdatedById = userId,
                    UpdatedAt = now
                });
            }
        }

        private static async Task SetNumberValueAsync(ApplicationDbContext context, AssociationInstance associationInstance, AssociationPropertyType propertyType, string? rawValue, string userId, DateTime now)
        {
            var existing = await context.AssociationPropertyValueNumbers.FirstOrDefaultAsync(v => v.AssociationInstanceId == associationInstance.Id && v.AssociationPropertyTypeId == propertyType.Id);

            if (string.IsNullOrWhiteSpace(rawValue))
            {
                if (existing != null)
                {
                    context.AssociationPropertyValueNumbers.Remove(existing);
                }
                return;
            }

            if (!PropertyValueParser.TryParseNumber(rawValue, out var value, out _))
            {
                return;
            }

            if (existing != null)
            {
                existing.Value = value;
                existing.UpdatedById = userId;
                existing.UpdatedAt = now;
            }
            else
            {
                context.AssociationPropertyValueNumbers.Add(new AssociationPropertyValueNumber
                {
                    AssociationInstanceId = associationInstance.Id,
                    AssociationPropertyTypeId = propertyType.Id,
                    Value = value,
                    CreatedById = userId,
                    CreatedAt = now,
                    UpdatedById = userId,
                    UpdatedAt = now
                });
            }
        }

        private static async Task SetDateTimeValueAsync(ApplicationDbContext context, AssociationInstance associationInstance, AssociationPropertyType propertyType, string? rawValue, string userId, DateTime now)
        {
            var existing = await context.AssociationPropertyValueDateTimes.FirstOrDefaultAsync(v => v.AssociationInstanceId == associationInstance.Id && v.AssociationPropertyTypeId == propertyType.Id);

            if (string.IsNullOrWhiteSpace(rawValue))
            {
                if (existing != null)
                {
                    context.AssociationPropertyValueDateTimes.Remove(existing);
                }
                return;
            }

            if (!PropertyValueParser.TryParseDateTime(rawValue, out var value, out _))
            {
                return;
            }

            if (existing != null)
            {
                existing.Value = value;
                existing.UpdatedById = userId;
                existing.UpdatedAt = now;
            }
            else
            {
                context.AssociationPropertyValueDateTimes.Add(new AssociationPropertyValueDateTime
                {
                    AssociationInstanceId = associationInstance.Id,
                    AssociationPropertyTypeId = propertyType.Id,
                    Value = value,
                    CreatedById = userId,
                    CreatedAt = now,
                    UpdatedById = userId,
                    UpdatedAt = now
                });
            }
        }
    }
}
