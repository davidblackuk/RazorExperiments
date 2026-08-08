namespace Wyrm.ViewModels
{
    /// <summary>
    /// Holds the user-entered fields for creating or editing a repository.
    /// Audit fields are stamped server-side, not collected here.
    /// </summary>
    public class RepositoryFormInput
    {
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
