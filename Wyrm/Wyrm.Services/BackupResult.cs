namespace Wyrm.Services
{
    public record BackupResult(bool Success, string? ErrorMessage, string? FileName)
    {
        public static BackupResult Ok(string fileName) => new(true, null, fileName);

        public static BackupResult Fail(string errorMessage) => new(false, errorMessage, null);
    }
}
