using Wyrm.Models;

namespace Wyrm.ViewModels
{
    /// <summary>
    /// Represents one dynamically-rendered property field on an instance create/edit form.
    /// Only PropertyTypeId and RawValue are trusted from a postback - Name/Description/DataType
    /// are always re-loaded from the database server-side before parsing or redisplay.
    /// </summary>
    public class PropertyFieldInput
    {
        public int PropertyTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PropertyDataType DataType { get; set; }
        public string? RawValue { get; set; }
    }
}
