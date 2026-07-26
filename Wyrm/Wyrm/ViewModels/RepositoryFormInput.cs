namespace Wyrm.ViewModels
{
    /// <summary>
    /// Holds the user-entered fields for creating a repository from the Explorer toolbar.
    /// Audit fields are stamped server-side, not collected here.
    /// </summary>
    public class RepositoryFormInput
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
