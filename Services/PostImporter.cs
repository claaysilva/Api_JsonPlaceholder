using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using JsonPlaceholderApi.Models;
using JsonPlaceholderApi.Data;
using Microsoft.EntityFrameworkCore;

namespace JsonPlaceholderApi.Services
{
  public class PostImporter : IPostImporter
  {
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AppDbContext _context;
    private readonly Microsoft.Extensions.Logging.ILogger<PostImporter> _logger;

    public PostImporter(IHttpClientFactory httpClientFactory, AppDbContext context, Microsoft.Extensions.Logging.ILogger<PostImporter> logger)
    {
      _httpClientFactory = httpClientFactory;
      _context = context;
      _logger = logger;
    }

    public async Task<IEnumerable<Post>> FetchAndImportAsync()
    {
      var client = _httpClientFactory.CreateClient();
      try
      {
        var response = await client.GetAsync("https://jsonplaceholder.typicode.com/posts");
        response.EnsureSuccessStatusCode();

        var posts = await response.Content.ReadFromJsonAsync<List<Post>>();
        if (posts == null) return new List<Post>();

        foreach (var post in posts)
        {
          if (!await _context.Posts.AnyAsync(p => p.Id == post.Id))
          {
            _context.Posts.Add(post);
          }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Imported {Count} posts from external API.", posts.Count);
        return posts;
      }
      catch (HttpRequestException ex)
      {
        _logger.LogError(ex, "Error fetching posts from external API.");
        throw;
      }
      catch (System.Exception ex)
      {
        _logger.LogError(ex, "Unexpected error importing posts.");
        throw;
      }
    }
  }
}
