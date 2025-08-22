namespace JsonPlaceholderApi.Dtos
{
  public class PostDto
  {
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
  }
}
