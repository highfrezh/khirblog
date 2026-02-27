using BlogApp.Repositories.Interfaces;
using BlogApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    public class CategoryController : BaseController  // ← change this
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBlogPostRepository _blogPostRepository;

        public CategoryController(
            ICategoryRepository categoryRepository,
            IBlogPostRepository blogPostRepository)
            : base(categoryRepository)  // ← add this
        {
            _categoryRepository = categoryRepository;
            _blogPostRepository = blogPostRepository;
        }

        // GET: /Category/Index
        public async Task<IActionResult> Index(int id, int page = 1)
        {
            int pageSize = 9;
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return NotFound();
            var posts = await _blogPostRepository.GetByCategoryAsync(id);
            var postList = posts.ToList();
            var totalPosts = postList.Count;
            var pagedPosts = postList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            var viewModel = new CategoryDetailViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                TotalPosts = totalPosts,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalPosts / (double)pageSize),
                Posts = pagedPosts.Select(p => new BlogPostViewModel
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
                }).ToList()
            };
            return View(viewModel);
        }

        // GET: /Category/All
        public async Task<IActionResult> All()
        {
            var categories = await _categoryRepository.GetAllAsync();

            var viewModel = categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                PostCount = c.BlogPosts.Count
            }).ToList();

            return View(viewModel);
        }
    }
}