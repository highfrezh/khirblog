using BlogApp.Repositories.Interfaces;
using BlogApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;

        public HomeController(
            IBlogPostRepository blogPostRepository,
            ICategoryRepository categoryRepository,
            ITagRepository tagRepository)
            : base(categoryRepository) 
        {
            _blogPostRepository = blogPostRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
        }

        // GET: /Home/Index
        public async Task<IActionResult> Index()
        {
            var posts = await _blogPostRepository.GetPublishedAsync();
            var categories = await _categoryRepository.GetAllAsync();
            var tags = await _tagRepository.GetAllAsync();
            var postList = posts.ToList();
            var categoryList = categories.ToList();
            var tagList = tags.ToList();
            var viewModel = new HomeViewModel
            {
                FeaturedPost = postList
                    .OrderByDescending(p => p.Likes.Count)
                    .Select(p => new BlogPostViewModel
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Slug = p.Slug,
                        Excerpt = p.Excerpt,
                        ImageUrl = p.ImageUrl,
                        AuthorName = $"{p.Author.FirstName} {p.Author.LastName}",
                        CreatedAt = p.CreatedAt,
                        CategoryName = p.Category.Name,
                        Tags = p.BlogPostTags.Select(bt => bt.Tag.Name).ToList(),
                        CommentCount = p.Comments.Count,
                        LikeCount = p.Likes.Count
                    })
                    .FirstOrDefault(),
                RecentPosts = postList
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(6)
                    .Select(p => new BlogPostViewModel
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Slug = p.Slug,
                        Excerpt = p.Excerpt,
                        ImageUrl = p.ImageUrl,
                        AuthorName = $"{p.Author.FirstName} {p.Author.LastName}",
                        CreatedAt = p.CreatedAt,
                        CategoryName = p.Category.Name,
                        Tags = p.BlogPostTags.Select(bt => bt.Tag.Name).ToList(),
                        CommentCount = p.Comments.Count,
                        LikeCount = p.Likes.Count
                    })
                    .ToList(),
                Categories = categoryList
                    .Select(c => new CategoryViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        PostCount = c.BlogPosts.Count
                    })
                    .ToList(),
                Tags = tagList
                    .Select(t => new TagViewModel
                    {
                        Id = t.Id,
                        Name = t.Name
                    })
                    .ToList()
            };
            return View(viewModel);
        }

        // GET: /Home/Error
        public IActionResult Error()
        {
            return View();
        }
    }
}