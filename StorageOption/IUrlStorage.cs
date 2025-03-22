namespace UrlShortener
{
    public interface IUrlStorage
    {
        Task<string?> GetOriginalUrlAsync(string shortKey);
        Task<string> StoreUrlAsync(string originalUrl);
    }
}
