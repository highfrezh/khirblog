namespace BlogApp.ViewModels
{
    public class CategoryDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int TotalPosts { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public List<BlogPostViewModel> Posts { get; set; } = new();
    }
}