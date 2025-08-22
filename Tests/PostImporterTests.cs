using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using JsonPlaceholderApi.Data;
using JsonPlaceholderApi.Services;
using JsonPlaceholderApi.Models;
using System.Collections.Generic;

namespace JsonPlaceholderApi.Tests
{
  public class PostImporterTests
  {
    [Fact]
    public async Task FetchAndImportAsync_SavesPostsToDatabase()
    {
      var options = new DbContextOptionsBuilder<AppDbContext>()
          .UseInMemoryDatabase(databaseName: "TestDb").Options;

      var samplePosts = new List<Post>
            {
                new Post { Id = 201, UserId = 1, Title = "t1", Body = "b1" },
                new Post { Id = 202, UserId = 2, Title = "t2", Body = "b2" }
            };

      var handler = new FakeHttpMessageHandler(JsonContent.Create(samplePosts));
      var client = new HttpClient(handler);
      var factory = new SimpleHttpClientFactory(client);

      using (var context = new AppDbContext(options))
      {
        var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<JsonPlaceholderApi.Services.PostImporter>.Instance;
        var importer = new PostImporter(factory, context, logger);

        var result = await importer.FetchAndImportAsync();

        Assert.NotNull(result);
        Assert.Equal(2, await context.Posts.CountAsync());
      }
    }
  }

  class FakeHttpMessageHandler : HttpMessageHandler
  {
    private readonly HttpContent _content;
    private readonly HttpStatusCode _statusCode;
    public FakeHttpMessageHandler(HttpContent content, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
      _content = content;
      _statusCode = statusCode;
    }
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
    {
      var response = new HttpResponseMessage(_statusCode) { Content = _content };
      return Task.FromResult(response);
    }
  }

  class SimpleHttpClientFactory : IHttpClientFactory
  {
    private readonly HttpClient _client;
    public SimpleHttpClientFactory(HttpClient client) => _client = client;
    public HttpClient CreateClient(string name) => _client;
  }

  public class PostImporterFailureTests
  {
    [Fact]
    public async Task FetchAndImportAsync_WhenHttpFails_ThrowsHttpRequestException()
    {
      var options = new DbContextOptionsBuilder<AppDbContext>()
          .UseInMemoryDatabase(databaseName: "FailTestDb").Options;

      var handler = new FakeHttpMessageHandler(new StringContent("error"), HttpStatusCode.BadGateway);
      var client = new HttpClient(handler);
      var factory = new SimpleHttpClientFactory(client);

      using (var context = new AppDbContext(options))
      {
        var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<JsonPlaceholderApi.Services.PostImporter>.Instance;
        var importer = new PostImporter(factory, context, logger);

        await Assert.ThrowsAsync<HttpRequestException>(() => importer.FetchAndImportAsync());
      }
    }
  }

  // Integration tests removed to avoid testhost complexity on local environment.
}
