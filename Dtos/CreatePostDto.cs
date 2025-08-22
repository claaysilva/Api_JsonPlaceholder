using System.ComponentModel.DataAnnotations;

namespace JsonPlaceholderApi.Dtos
{
  public class CreatePostDto
  {
    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;
  }
}
