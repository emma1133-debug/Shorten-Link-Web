using UrlShortener;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using UrlShortener.StorageOption.EF;
using UrlShortener.StorageOption;


var builder = WebApplication.CreateBuilder(args);

// 👉 CORS cho frontend (React)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

// 👉 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "URL Shortener API",
        Version = "v1",
        Description = "A simple API to shorten URLs"
    });
});

// 👉 Controllers
builder.Services.AddControllers();

// 👉 PostgreSQL Connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<UrlDbContext>(options =>
    options.UseNpgsql(connectionString));

// 👉 Dùng EF với PostgreSQL
builder.Services.AddScoped<IUrlStorage, PostgresUrlStorage>();

var app = builder.Build();

app.UseCors("AllowReactApp");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization(); // ❗ Nếu không dùng Auth thì có thể bỏ dòng này

app.MapControllers();

// 👉 Dùng đúng cổng như trong docker-compose.yml
app.Urls.Add("http://0.0.0.0:5050");

app.Run();
