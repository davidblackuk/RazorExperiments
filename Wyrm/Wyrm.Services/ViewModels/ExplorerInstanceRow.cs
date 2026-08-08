namespace Wyrm.ViewModels
{
    /// <summary>
    /// One row in the Explorer instance grid - the flattened fields the grid partial needs to render,
    /// so the partial doesn't have to re-derive display name / audit user names itself.
    /// </summary>
    public class ExplorerInstanceRow
    {
        public required int Id { get; set; }
        public required string DisplayName { get; set; }
        public string? CreatedByUserName { get; set; }
        public string? UpdatedByUserName { get; set; }
    }
}
