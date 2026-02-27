using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

public class BlogPostFormViewModel
{
    public int Id { get; set; } // needed for Edit

    [Required]
    [StringLength(200)]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    [StringLength(500)]
    public string? Excerpt { get; set; }

    public string? Slug { get; set; }

    public string? ImageUrl { get; set; } // existing image (used in Edit)

    [Display(Name = "Upload Image")]
    public IFormFile? ImageFile { get; set; } // actual file upload

    public bool IsPublished { get; set; } = true;

    [Required]
    public int CategoryId { get; set; }

    public List<int> SelectedTagIds { get; set; } = new List<int>();

    // Dropdowns for the form
    public IEnumerable<SelectListItem>? Categories { get; set; }
    public IEnumerable<SelectListItem>? Tags { get; set; }
}