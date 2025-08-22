using JsonPlaceholderApi.Data;
using Microsoft.EntityFrameworkCore;
using JsonPlaceholderApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Conexão MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;User=root;Database=jsonplaceholderdb;";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
// Register HttpClient and importer service
builder.Services.AddHttpClient();
builder.Services.AddScoped<IPostImporter, PostImporter>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Global exception middleware
app.UseMiddleware<JsonPlaceholderApi.Middleware.ExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

// Expose Program class for integration tests
public partial class Program { }
