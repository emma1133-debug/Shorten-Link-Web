using UrlShortener.StorageOption.EF;
using Microsoft.EntityFrameworkCore;

namespace UrlShortener.StorageOption
{
    public class PostgresUrlStorage : IUrlStorage
    {
        private readonly UrlDbContext _context;

        public PostgresUrlStorage(UrlDbContext context)
        {
            _context = context;
        }

        public async Task<string> StoreUrlAsync(string originalUrl)
        {
            var shortKey = Guid.NewGuid().ToString("N")[..8]; // random shortKey
            var mapping = new UrlMapping
            {
                ShortKey = shortKey,
                OriginalUrl = originalUrl
            };

            _context.UrlMappings.Add(mapping);
            await _context.SaveChangesAsync();
            return shortKey;
        }

        public async Task<string?> GetOriginalUrlAsync(string shortKey)
        {
            var mapping = await _context.UrlMappings
                .FirstOrDefaultAsync(m => m.ShortKey == shortKey);

            return mapping?.OriginalUrl;
        }
    }
}
