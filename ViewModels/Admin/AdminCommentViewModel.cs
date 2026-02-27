namespace BlogApp.ViewModels.Admin
{
    public class AdminCommentViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string AuthorName { get; set; }
        public string PostTitle { get; set; }
        public string PostSlug { get; set; }
        public int BlogPostId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}