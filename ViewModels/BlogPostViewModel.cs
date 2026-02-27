public class BlogPostViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Excerpt { get; set; }
    public string AuthorName { get; set; }
    public string Slug { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CategoryName { get; set; } // added
    public List<string> Tags { get; set; } = new List<string>();
    public int CommentCount { get; set; }
    public int LikeCount { get; set; }
}