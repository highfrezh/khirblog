namespace BlogApp.ViewModels.Admin
{
    public class AdminBlogPostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string AuthorName { get; set; }
        public string CategoryName { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
    }
}