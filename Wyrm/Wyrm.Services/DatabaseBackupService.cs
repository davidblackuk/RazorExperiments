namespace Wyrm.Services
{
    public class DatabaseBackupService(string databaseFilePath) : IDatabaseBackupService
    {
        public Task<BackupResult> BackupAsync()
        {
            if (!File.Exists(databaseFilePath))
            {
                return Task.FromResult(BackupResult.Fail($"Database file not found at '{databaseFilePath}'."));
            }

            var directory = Path.GetDirectoryName(databaseFilePath)!;
            var baseName = Path.GetFileNameWithoutExtension(databaseFilePath);
            var extension = Path.GetExtension(databaseFilePath);
            var timestamp = DateTime.Now.ToString("yyyy.MM.dd.HH.mm");
            var backupFileName = $"{baseName}.{timestamp}{extension}";
            var backupPath = Path.Combine(directory, backupFileName);

            try
            {
                File.Copy(databaseFilePath, backupPath, overwrite: false);
            }
            catch (Exception ex)
            {
                return Task.FromResult(BackupResult.Fail($"Backup failed: {ex.Message}"));
            }

            return Task.FromResult(BackupResult.Ok(backupFileName));
        }
    }
}
