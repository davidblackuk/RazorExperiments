namespace Wyrm.Services
{
    /// <summary>
    /// Names of the auto-seeded system PropertyTypes (see CreateObjectType.cshtml.cs) that mirror an
    /// ObjectInstance's own audit fields. These are never user-editable on the instance form - their
    /// values are stamped automatically from ObjectInstance.CreatedBy/CreatedAt/UpdatedBy/UpdatedAt.
    /// </summary>
    public static class SystemPropertyNames
    {
        public const string WhoCreated = "Who Created";
        public const string WhenCreated = "When Created";
        public const string WhoUpdated = "Who Updated";
        public const string WhenUpdated = "When Updated";

        private static readonly HashSet<string> AuditMirrorNames = new()
        {
            WhoCreated, WhenCreated, WhoUpdated, WhenUpdated
        };

        public static bool IsAuditMirror(string name) => AuditMirrorNames.Contains(name);
    }
}
