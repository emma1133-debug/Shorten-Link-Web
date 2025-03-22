namespace UrlShortener.StorageOption.EF
{
    public class UrlMapping
    {
        public int Id { get; set; }
        public string ShortKey { get; set; } = null!;
        public string OriginalUrl { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // ✅ Thêm thời gian tạo
        public string? Title { get; set; } // ✅ Optional title
    }
}
