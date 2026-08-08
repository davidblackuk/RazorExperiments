namespace Wyrm.Services
{
    public record ServiceResult(bool Success, string? ErrorMessage)
    {
        public static ServiceResult Ok() => new(true, null);

        public static ServiceResult Fail(string errorMessage) => new(false, errorMessage);
    }
}
