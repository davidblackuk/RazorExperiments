namespace Wyrm.ViewModels
{
    /// <summary>
    /// Holds the user-entered fields for creating or editing an object type from the Designer.
    /// Audit fields and the auto-seeded system property types are stamped server-side, not collected here.
    /// </summary>
    public class ObjectTypeFormInput
    {
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
