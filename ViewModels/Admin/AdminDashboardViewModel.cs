namespace BlogApp.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalPosts { get; set; }
        public int PublishedPosts { get; set; }
        public int DraftPosts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalTags { get; set; }
        public int TotalUsers { get; set; }
        public int TotalLikes { get; set; }
        public int TotalComments { get; set; }
        public List<AdminRecentPostViewModel> RecentPosts { get; set; } = new();
    }

    public class AdminRecentPostViewModel
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