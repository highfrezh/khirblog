using BlogApp.Repositories.Interfaces;
using BlogApp.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Dashboard/[action]")]
    public class AdminDashboardController : Controller
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICommentRepository _commentRepository;

        public AdminDashboardController(
            IBlogPostRepository blogPostRepository,
            ICategoryRepository categoryRepository,
            ITagRepository tagRepository,
            IUserRepository userRepository,
            ICommentRepository commentRepository)
        {
            _blogPostRepository = blogPostRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            _userRepository = userRepository;
            _commentRepository = commentRepository;
        }

        // GET: /Admin/AdminDashboard/Index
        public async Task<IActionResult> Index()
        {
            var posts = await _blogPostRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();
            var tags = await _tagRepository.GetAllAsync();
            var users = await _userRepository.GetAllAsync();

            var viewModel = new AdminDashboardViewModel
            {
                TotalPosts = posts.Count(),
                PublishedPosts = posts.Count(p => p.IsPublished),
                DraftPosts = posts.Count(p => !p.IsPublished),
                TotalCategories = categories.Count(),
                TotalTags = tags.Count(),
                TotalUsers = users.Count(),
                TotalLikes = posts.Sum(p => p.Likes.Count),
                TotalComments = posts.Sum(p => p.Comments.Count),
                RecentPosts = posts
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
                    .Select(p => new AdminRecentPostViewModel
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Slug = p.Slug,
                        AuthorName = $"{p.Author.FirstName} {p.Author.LastName}",
                        CategoryName = p.Category.Name,
                        IsPublished = p.IsPublished,
                        CreatedAt = p.CreatedAt,
                        LikeCount = p.Likes.Count,
                        CommentCount = p.Comments.Count
                    })
                    .ToList()
            };

            return View(viewModel);
        }
    }
}