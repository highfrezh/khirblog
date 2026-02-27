namespace BlogApp.ViewModels
{
    public class BlogPostIndexViewModel
    {
        public List<BlogPostViewModel> Posts { get; set; } = new();
        public List<CategoryViewModel> Categories { get; set; } = new();
        public List<TagViewModel> Tags { get; set; } = new();
        public string? SearchKeyword { get; set; }
        public int? SelectedCategoryId { get; set; }
        public int? SelectedTagId { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
    }
}