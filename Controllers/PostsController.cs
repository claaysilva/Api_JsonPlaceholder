using Microsoft.AspNetCore.Mvc;
using JsonPlaceholderApi.Data;
using JsonPlaceholderApi.Models;
using Microsoft.EntityFrameworkCore;
using JsonPlaceholderApi.Services;
using JsonPlaceholderApi.Dtos;

namespace JsonPlaceholderApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class PostsController : ControllerBase
  {
    private readonly AppDbContext _context;
    private readonly IPostImporter _importer;

    public PostsController(AppDbContext context, IPostImporter importer)
    {
      _context = context;
      _importer = importer;
    }

    // GET: api/posts/fetch
    [HttpGet("fetch")]
    public async Task<IActionResult> FetchAndSavePosts()
    {
      try
      {
        var posts = await _importer.FetchAndImportAsync();
        return Ok(new { message = "Posts importados e salvos com sucesso.", imported = posts?.Count() ?? 0 });
      }
      catch (HttpRequestException)
      {
        return StatusCode(502, "Erro ao buscar dados da API externa.");
      }
    }

    // GET: api/posts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetPosts()
    {
      var posts = await _context.Posts.Select(p => new PostDto { Id = p.Id, UserId = p.UserId, Title = p.Title, Body = p.Body }).ToListAsync();
      return Ok(posts);
    }

    // GET: api/posts/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<PostDto>> GetPost(int id)
    {
      var post = await _context.Posts.FindAsync(id);
      if (post == null) return NotFound();
      return Ok(new PostDto { Id = post.Id, UserId = post.UserId, Title = post.Title, Body = post.Body });
    }

    // POST: api/posts
    [HttpPost]
    public async Task<ActionResult<PostDto>> CreatePost([FromBody] CreatePostDto dto)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var post = new Post { UserId = dto.UserId, Title = dto.Title, Body = dto.Body };
      _context.Posts.Add(post);
      await _context.SaveChangesAsync();

      var result = new PostDto { Id = post.Id, UserId = post.UserId, Title = post.Title, Body = post.Body };
      return CreatedAtAction(nameof(GetPost), new { id = post.Id }, result);
    }

    // PUT: api/posts/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(int id, [FromBody] CreatePostDto dto)
    {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var post = await _context.Posts.FindAsync(id);
      if (post == null) return NotFound();

      post.UserId = dto.UserId;
      post.Title = dto.Title;
      post.Body = dto.Body;

      _context.Posts.Update(post);
      await _context.SaveChangesAsync();
      return NoContent();
    }

    // DELETE: api/posts/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
      var post = await _context.Posts.FindAsync(id);
      if (post == null) return NotFound();
      _context.Posts.Remove(post);
      await _context.SaveChangesAsync();
      return NoContent();
    }
  }
}
