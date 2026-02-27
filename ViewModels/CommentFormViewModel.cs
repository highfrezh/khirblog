using System.ComponentModel.DataAnnotations;

public class CommentFormViewModel
{
    [Required]
    [StringLength(1000)]
    public string Content { get; set; }

    public int BlogPostId { get; set; }
}
