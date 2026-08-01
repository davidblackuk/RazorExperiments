using System.Globalization;
using Wyrm.Models;

namespace Wyrm.Services
{
    /// <summary>
    /// Validates and converts the raw string values posted from a dynamic property form
    /// into the typed values that <see cref="PropertyValueStore"/> persists. Does not touch the database.
    /// </summary>
    public static class PropertyValueParser
    {
        /// <summary>
        /// Validates that <paramref name="rawValue"/> can be parsed as <paramref name="dataType"/>.
        /// A blank value is always valid (it means "no value set" - there is no IsRequired flag on PropertyType today).
        /// </summary>
        public static bool TryValidate(PropertyDataType dataType, string? rawValue, out string? error)
        {
            error = null;

            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return true;
            }

            return dataType switch
            {
                PropertyDataType.Int => TryParseInt(rawValue, out _, out error),
                PropertyDataType.Number => TryParseNumber(rawValue, out _, out error),
                PropertyDataType.Date or PropertyDataType.DateTime => TryParseDateTime(rawValue, out _, out error),
                _ => true
            };
        }

        public static bool TryParseInt(string rawValue, out int value, out string? error)
        {
            if (int.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                error = null;
                return true;
            }

            error = "Enter a whole number.";
            return false;
        }

        public static bool TryParseNumber(string rawValue, out double value, out string? error)
        {
            if (double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            {
                error = null;
                return true;
            }

            error = "Enter a number.";
            return false;
        }

        public static bool TryParseDateTime(string rawValue, out DateTime value, out string? error)
        {
            if (DateTime.TryParse(rawValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out value))
            {
                error = null;
                return true;
            }

            error = "Enter a valid date.";
            return false;
        }
    }
}
