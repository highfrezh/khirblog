namespace BlogApp.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string AuthorName { get; set; }
        public string AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}