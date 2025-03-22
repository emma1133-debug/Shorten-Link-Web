using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UrlShortener;

public class UrlShortenerService
{
    private readonly IUrlStorage _urlStorage;
    private readonly string _baseUrl = "http://localhost:5158/";

    public UrlShortenerService(IUrlStorage urlStorage)
    {
        _urlStorage = urlStorage;
    }

    public async Task<string> ShortenUrlAsync(string originalUrl)
    {
        var shortKey = await GenerateUniqueShortKeyAsync();
        await _urlStorage.StoreUrlAsync(originalUrl); // Lưu theo chuẩn
        return shortKey;
    }

    public async Task<string?> GetOriginalUrlAsync(string shortKey)
    {
        return await _urlStorage.GetOriginalUrlAsync(shortKey);
    }

    private async Task<string> GenerateUniqueShortKeyAsync()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        string shortKey;
        bool isUnique;

        do
        {
            shortKey = new string(Enumerable.Range(0, 6)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());

            var existingUrl = await _urlStorage.GetOriginalUrlAsync(shortKey);
            isUnique = string.IsNullOrEmpty(existingUrl);

        } while (!isUnique);

        return shortKey;
    }
}
