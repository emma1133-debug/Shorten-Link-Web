using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Models;
using UrlShortener.StorageOption.EF;

[Route("api")]
[ApiController]
public class UrlController : ControllerBase
{
    private readonly UrlDbContext _context;
    private readonly HttpClient _httpClient;

    public UrlController(UrlDbContext context)
    {
        _context = context;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
    }

    // API lấy danh sách URL đã rút gọn
    [HttpGet("urls")]
    public async Task<IActionResult> GetShortenedUrls()
    {
        var urls = await _context.UrlMappings
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new
            {
                u.ShortKey,
                u.OriginalUrl,
                Title = u.Title ?? "No Title",
                CreatedAt = u.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            })
            .ToListAsync();

        return Ok(urls);
    }

    // API rút gọn URL
    [HttpPost("url/shorten")]
    public async Task<IActionResult> ShortenUrl([FromBody] UrlRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.OriginalUrl))
            return BadRequest(new { error = "Invalid URL: URL cannot be empty or whitespace." });

        if (!await IsValidUrl(request.OriginalUrl))
            return BadRequest(new { error = "The provided URL is either inaccessible or invalid." });

        var shortKey = Guid.NewGuid().ToString().Substring(0, 6);
        var createdAt = DateTime.UtcNow;
        var title = await GetPageTitleAsync(request.OriginalUrl);

        var urlMapping = new UrlMapping
        {
            ShortKey = shortKey,
            OriginalUrl = request.OriginalUrl,
            CreatedAt = createdAt,
            Title = title
        };

        _context.UrlMappings.Add(urlMapping);
        await _context.SaveChangesAsync();

        return Ok(new { shortKey, createdAt, title });
    }

    // API chuyển hướng từ short URL sang URL gốc
    [HttpGet("url/{shortKey}")]
    public async Task<IActionResult> RedirectToOriginalUrl(string shortKey)
    {
        var urlMapping = await _context.UrlMappings.FirstOrDefaultAsync(u => u.ShortKey == shortKey);

        if (urlMapping == null)
        {
            return NotFound(new { error = "Shortened URL not found." });
        }

        return Redirect(urlMapping.OriginalUrl);
    }

    // Kiểm tra xem URL có tồn tại hay không
    private async Task<bool> IsValidUrl(string url)
    {
        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

            var response = await httpClient.GetAsync(url);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"URL validation error: {ex.Message}");
            return false;
        }
    }

    // Lấy tiêu đề trang web
    private async Task<string> GetPageTitleAsync(string url)
    {
        try
        {
            var response = await _httpClient.GetStringAsync(url);
            var match = Regex.Match(response, @"<title>(.*?)</title>", RegexOptions.IgnoreCase);

            return match.Success ? match.Groups[1].Value.Trim() : "No Title";
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching title for {url}: {ex.Message}");
            return "No Title";
        }
    }
}
