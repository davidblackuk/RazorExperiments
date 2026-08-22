namespace Wyrm.ViewModels
{
    /// <summary>
    /// Holds the user-entered fields for creating or editing an association type from the Designer.
    /// Audit fields are stamped server-side, not collected here.
    /// </summary>
    public class AssociationTypeFormInput
    {
        public int? Id { get; set; }
        public string ForwardName { get; set; } = string.Empty;
        public string ReverseName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        /// <summary>Null means "Any" object type.</summary>
        public int? SourceObjectTypeId { get; set; }

        /// <summary>Null means "Any" object type.</summary>
        public int? TargetObjectTypeId { get; set; }
    }
}
