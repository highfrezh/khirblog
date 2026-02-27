namespace BlogApp.ViewModels
{
    public class HomeViewModel
    {
        public BlogPostViewModel? FeaturedPost { get; set; }
        public List<BlogPostViewModel> RecentPosts { get; set; } = new();
        public List<CategoryViewModel> Categories { get; set; } = new();
        public List<TagViewModel> Tags { get; set; } = new();
    }
}