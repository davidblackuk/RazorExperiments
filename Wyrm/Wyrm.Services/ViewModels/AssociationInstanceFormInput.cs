namespace Wyrm.ViewModels
{
    /// <summary>
    /// Holds the user-selected fields for creating a new association instance from the Explorer's
    /// "Associated Objects" tab. Audit fields are stamped server-side, not collected here.
    /// </summary>
    public class AssociationInstanceFormInput
    {
        public int AssociationTypeId { get; set; }

        /// <summary>Whether the object instance the user was viewing plays the Source role in the new link.</summary>
        public bool CurrentInstanceIsSource { get; set; }

        public int CurrentInstanceId { get; set; }
        public int OtherInstanceId { get; set; }
        public List<PropertyFieldInput> PropertyFields { get; set; } = new();
    }
}
