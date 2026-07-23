using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Wyrm.Data;
using Wyrm.Models;

namespace Wyrm.Services
{
    /// <summary>
    /// Dispatches property value reads/writes to the correct one of the four value tables
    /// (PropertyValueStrings/Ints/Numbers/DateTimes) based on a PropertyType's DataType,
    /// so page code doesn't need to repeat that branch.
    /// </summary>
    public static class PropertyValueStore
    {
        /// <summary>
        /// Loads the existing values for an object instance as a flat PropertyTypeId -> display/edit string map.
        /// A missing key means no value has been recorded for that property yet.
        /// </summary>
        public static async Task<Dictionary<int, string?>> LoadRawValuesAsync(ApplicationDbContext context, int objectInstanceId, IEnumerable<PropertyType> propertyTypes)
        {
            var propertyTypeList = propertyTypes.ToList();
            var result = new Dictionary<int, string?>();

            var stringIds = propertyTypeList.Where(pt => pt.DataType is PropertyDataType.String or PropertyDataType.Memo).Select(pt => pt.Id).ToList();
            var intIds = propertyTypeList.Where(pt => pt.DataType == PropertyDataType.Int).Select(pt => pt.Id).ToList();
            var numberIds = propertyTypeList.Where(pt => pt.DataType == PropertyDataType.Number).Select(pt => pt.Id).ToList();
            var dateTimeIds = propertyTypeList.Where(pt => pt.DataType is PropertyDataType.Date or PropertyDataType.DateTime).Select(pt => pt.Id).ToList();

            foreach (var row in await context.PropertyValueStrings.Where(v => v.ObjectInstanceId == objectInstanceId && stringIds.Contains(v.PropertyTypeId)).ToListAsync())
            {
                result[row.PropertyTypeId] = row.Value;
            }

            foreach (var row in await context.PropertyValueInts.Where(v => v.ObjectInstanceId == objectInstanceId && intIds.Contains(v.PropertyTypeId)).ToListAsync())
            {
                result[row.PropertyTypeId] = row.Value.ToString(CultureInfo.InvariantCulture);
            }

            foreach (var row in await context.PropertyValueNumbers.Where(v => v.ObjectInstanceId == objectInstanceId && numberIds.Contains(v.PropertyTypeId)).ToListAsync())
            {
                result[row.PropertyTypeId] = row.Value.ToString(CultureInfo.InvariantCulture);
            }

            var dataTypeByPropertyTypeId = propertyTypeList.ToDictionary(pt => pt.Id, pt => pt.DataType);
            foreach (var row in await context.PropertyValueDateTimes.Where(v => v.ObjectInstanceId == objectInstanceId && dateTimeIds.Contains(v.PropertyTypeId)).ToListAsync())
            {
                var format = dataTypeByPropertyTypeId[row.PropertyTypeId] == PropertyDataType.Date ? "yyyy-MM-dd" : "yyyy-MM-ddTHH:mm";
                result[row.PropertyTypeId] = row.Value.ToString(format, CultureInfo.InvariantCulture);
            }

            return result;
        }

        /// <summary>
        /// Stamps the "Who Created"/"When Created"/"Who Updated"/"When Updated" system properties (see
        /// <see cref="SystemPropertyNames"/>) from the instance's own audit fields, rather than from
        /// user input. "Who/When Created" are only stamped on create, so they stay fixed afterwards.
        /// </summary>
        public static async Task SetAuditMirrorValuesAsync(ApplicationDbContext context, ObjectInstance instance, IEnumerable<PropertyType> propertyTypes, string userName, string userId, DateTime now, bool isCreate)
        {
            var whenText = now.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

            foreach (var propertyType in propertyTypes)
            {
                switch (propertyType.Name)
                {
                    case SystemPropertyNames.WhoCreated when isCreate:
                        await SetValueAsync(context, instance, propertyType, userName, userId, now);
                        break;
                    case SystemPropertyNames.WhenCreated when isCreate:
                        await SetValueAsync(context, instance, propertyType, whenText, userId, now);
                        break;
                    case SystemPropertyNames.WhoUpdated:
                        await SetValueAsync(context, instance, propertyType, userName, userId, now);
                        break;
                    case SystemPropertyNames.WhenUpdated:
                        await SetValueAsync(context, instance, propertyType, whenText, userId, now);
                        break;
                }
            }
        }

        /// <summary>
        /// Creates, updates, or removes the value row for (instance, propertyType) based on rawValue.
        /// A blank rawValue removes any existing row. The caller must have already validated rawValue
        /// with <see cref="PropertyValueParser.TryValidate"/>.
        /// </summary>
        public static async Task SetValueAsync(ApplicationDbContext context, ObjectInstance instance, PropertyType propertyType, string? rawValue, string userId, DateTime now)
        {
            switch (propertyType.DataType)
            {
                case PropertyDataType.String:
                case PropertyDataType.Memo:
                    await SetStringValueAsync(context, instance, propertyType, rawValue, userId, now);
                    break;
                case PropertyDataType.Int:
                    await SetIntValueAsync(context, instance, propertyType, rawValue, userId, now);
                    break;
                case PropertyDataType.Number:
                    await SetNumberValueAsync(context, instance, propertyType, rawValue, userId, now);
                    break;
                case PropertyDataType.Date:
                case PropertyDataType.DateTime:
                    await SetDateTimeValueAsync(context, instance, propertyType, rawValue, userId, now);
                    break;
            }
        }

        private static async Task SetStringValueAsync(ApplicationDbContext context, ObjectInstance instance, PropertyType propertyType, string? rawValue, string userId, DateTime now)
        {
            var existing = await context.PropertyValueStrings.FirstOrDefaultAsync(v => v.ObjectInstanceId == instance.Id && v.PropertyTypeId == propertyType.Id);

            if (string.IsNullOrEmpty(rawValue))
            {
                if (existing != null)
                {
                    context.PropertyValueStrings.Remove(existing);
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
                context.PropertyValueStrings.Add(new PropertyValueString
                {
                    ObjectInstanceId = instance.Id,
                    PropertyTypeId = propertyType.Id,
                    Value = rawValue,
                    CreatedById = userId,
                    CreatedAt = now,
                    UpdatedById = userId,
                    UpdatedAt = now
                });
            }
        }

        private static async Task SetIntValueAsync(ApplicationDbContext context, ObjectInstance instance, PropertyType propertyType, string? rawValue, string userId, DateTime now)
        {
            var existing = await context.PropertyValueInts.FirstOrDefaultAsync(v => v.ObjectInstanceId == instance.Id && v.PropertyTypeId == propertyType.Id);

            if (string.IsNullOrWhiteSpace(rawValue))
            {
                if (existing != null)
                {
                    context.PropertyValueInts.Remove(existing);
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
                context.PropertyValueInts.Add(new PropertyValueInt
                {
                    ObjectInstanceId = instance.Id,
                    PropertyTypeId = propertyType.Id,
                    Value = value,
                    CreatedById = userId,
                    CreatedAt = now,
                    UpdatedById = userId,
                    UpdatedAt = now
                });
            }
        }

        private static async Task SetNumberValueAsync(ApplicationDbContext context, ObjectInstance instance, PropertyType propertyType, string? rawValue, string userId, DateTime now)
        {
            var existing = await context.PropertyValueNumbers.FirstOrDefaultAsync(v => v.ObjectInstanceId == instance.Id && v.PropertyTypeId == propertyType.Id);

            if (string.IsNullOrWhiteSpace(rawValue))
            {
                if (existing != null)
                {
                    context.PropertyValueNumbers.Remove(existing);
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
                context.PropertyValueNumbers.Add(new PropertyValueNumber
                {
                    ObjectInstanceId = instance.Id,
                    PropertyTypeId = propertyType.Id,
                    Value = value,
                    CreatedById = userId,
                    CreatedAt = now,
                    UpdatedById = userId,
                    UpdatedAt = now
                });
            }
        }

        private static async Task SetDateTimeValueAsync(ApplicationDbContext context, ObjectInstance instance, PropertyType propertyType, string? rawValue, string userId, DateTime now)
        {
            var existing = await context.PropertyValueDateTimes.FirstOrDefaultAsync(v => v.ObjectInstanceId == instance.Id && v.PropertyTypeId == propertyType.Id);

            if (string.IsNullOrWhiteSpace(rawValue))
            {
                if (existing != null)
                {
                    context.PropertyValueDateTimes.Remove(existing);
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
                context.PropertyValueDateTimes.Add(new PropertyValueDateTime
                {
                    ObjectInstanceId = instance.Id,
                    PropertyTypeId = propertyType.Id,
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
