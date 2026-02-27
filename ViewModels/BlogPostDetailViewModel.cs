namespace BlogApp.ViewModels
{
    public class BlogPostDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }
        public string? Excerpt { get; set; }
        public string? ImageUrl { get; set; }
        public string AuthorName { get; set; }
        public string AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public List<string> Tags { get; set; } = new();
        public int LikeCount { get; set; }
        public bool HasLiked { get; set; }
        public List<CommentViewModel> Comments { get; set; } = new();
    }
}