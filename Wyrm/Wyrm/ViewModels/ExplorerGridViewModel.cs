using Wyrm.Models;

namespace Wyrm.ViewModels
{
    /// <summary>
    /// Model for the Explorer instance grid partial: the selected ObjectType plus its instances,
    /// pre-flattened into <see cref="ExplorerInstanceRow"/>.
    /// </summary>
    public class ExplorerGridViewModel
    {
        public required ObjectType ObjectType { get; set; }
        public required List<ExplorerInstanceRow> Rows { get; set; }
    }
}
