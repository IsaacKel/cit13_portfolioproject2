using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs
{
  public class BookmarkCreateDto
  {
    [Required]
    public int UserId { get; set; }
    public string TConst { get; set; }

    public string NConst { get; set; }

    [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters.")]
    public string Note { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
  }
}