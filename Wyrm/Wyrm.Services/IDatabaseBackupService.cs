namespace Wyrm.Services
{
    public interface IDatabaseBackupService
    {
        Task<BackupResult> BackupAsync();
    }
}
