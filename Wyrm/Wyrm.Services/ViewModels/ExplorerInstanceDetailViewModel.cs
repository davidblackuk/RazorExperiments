using Wyrm.Models;

namespace Wyrm.ViewModels
{
    /// <summary>
    /// Model for the Explorer instance detail partial: the selected ObjectInstance plus its
    /// resolved display name and raw property values, keyed by PropertyTypeId.
    /// </summary>
    public class ExplorerInstanceDetailViewModel
    {
        public required ObjectInstance ObjectInstance { get; set; }
        public required string DisplayName { get; set; }
        public required Dictionary<int, string?> Values { get; set; }
    }
}
