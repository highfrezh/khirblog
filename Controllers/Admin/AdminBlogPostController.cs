using BlogApp.Models;
using BlogApp.Repositories.Interfaces;
using BlogApp.Services.Interfaces;
using BlogApp.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]/[action]")]
    public class AdminBlogPostController : Controller
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IImageService _imageService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminBlogPostController(
            IBlogPostRepository blogPostRepository,
            ICategoryRepository categoryRepository,
            ITagRepository tagRepository,
            IImageService imageService,
            UserManager<ApplicationUser> userManager)
        {
            _blogPostRepository = blogPostRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            _imageService = imageService;
            _userManager = userManager;
        }

        // GET: /Admin/AdminBlogPost/Index
        public async Task<IActionResult> Index()
        {
            var posts = await _blogPostRepository.GetAllAsync();

            var viewModel = posts.Select(p => new AdminBlogPostViewModel
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
            }).ToList();

            return View(viewModel);
        }

        // GET: /Admin/AdminBlogPost/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new BlogPostFormViewModel();
            await PopulateDropdowns(viewModel);
            return View(viewModel);
        }

        // POST: /Admin/AdminBlogPost/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogPostFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var slug = GenerateSlug(model.Title);

            if (await _blogPostRepository.SlugExistsAsync(slug))
                slug = $"{slug}-{Guid.NewGuid().ToString()[..4]}";

            var blogPost = new BlogPost
            {
                Title = model.Title,
                Slug = slug,
                Content = model.Content,
                Excerpt = model.Excerpt,
                IsPublished = model.IsPublished,
                CategoryId = model.CategoryId,
                AuthorId = currentUser!.Id,
                CreatedAt = DateTime.UtcNow
            };

            if (model.ImageFile != null)
                blogPost.ImageUrl = await _imageService.UploadImageAsync(model.ImageFile);

            await _blogPostRepository.AddAsync(blogPost);

            if (model.SelectedTagIds != null && model.SelectedTagIds.Any())
            {
                foreach (var tagId in model.SelectedTagIds)
                {
                    blogPost.BlogPostTags.Add(new BlogPostTag
                    {
                        BlogPostId = blogPost.Id,
                        TagId = tagId
                    });
                }
                await _blogPostRepository.UpdateAsync(blogPost);
            }

            TempData["Success"] = "Post created successfully!";
            return RedirectToAction("Index");
        }

        // GET: /Admin/AdminBlogPost/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _blogPostRepository.GetByIdAsync(id);
            if (post == null) return NotFound();

            var viewModel = new BlogPostFormViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Slug = post.Slug,
                Content = post.Content,
                Excerpt = post.Excerpt,
                ImageUrl = post.ImageUrl,
                IsPublished = post.IsPublished,
                CategoryId = post.CategoryId,
                SelectedTagIds = post.BlogPostTags.Select(bt => bt.TagId).ToList()
            };

            await PopulateDropdowns(viewModel);
            return View(viewModel);
        }

        // POST: /Admin/AdminBlogPost/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BlogPostFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                return View(model);
            }

            var post = await _blogPostRepository.GetByIdAsync(model.Id);
            if (post == null) return NotFound();

            post.Title = model.Title;
            post.Content = model.Content;
            post.Excerpt = model.Excerpt;
            post.IsPublished = model.IsPublished;
            post.CategoryId = model.CategoryId;
            post.UpdatedAt = DateTime.UtcNow;

            if (model.ImageFile != null)
            {
                _imageService.DeleteImage(post.ImageUrl);
                post.ImageUrl = await _imageService.UploadImageAsync(model.ImageFile);
            }

            post.BlogPostTags.Clear();
            if (model.SelectedTagIds != null && model.SelectedTagIds.Any())
            {
                foreach (var tagId in model.SelectedTagIds)
                {
                    post.BlogPostTags.Add(new BlogPostTag
                    {
                        BlogPostId = post.Id,
                        TagId = tagId
                    });
                }
            }

            await _blogPostRepository.UpdateAsync(post);

            TempData["Success"] = "Post updated successfully!";
            return RedirectToAction("Index");
        }

        // POST: /Admin/AdminBlogPost/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _blogPostRepository.GetByIdAsync(id);
            if (post == null) return NotFound();

            _imageService.DeleteImage(post.ImageUrl);
            await _blogPostRepository.DeleteAsync(id);

            TempData["Success"] = "Post deleted successfully!";
            return RedirectToAction("Index");
        }

        // POST: /Admin/AdminBlogPost/TogglePublish/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePublish(int id)
        {
            var post = await _blogPostRepository.GetByIdAsync(id);
            if (post == null) return NotFound();

            post.IsPublished = !post.IsPublished;
            await _blogPostRepository.UpdateAsync(post);

            TempData["Success"] = post.IsPublished ? "Post published!" : "Post unpublished!";
            return RedirectToAction("Index");
        }

        private async Task PopulateDropdowns(BlogPostFormViewModel model)
        {
            var categories = await _categoryRepository.GetAllAsync();
            var tags = await _tagRepository.GetAllAsync();

            model.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });

            model.Tags = tags.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name
            });
        }

        private string GenerateSlug(string title)
        {
            return title.ToLower()
                .Replace(" ", "-")
                .Replace(".", "")
                .Replace(",", "")
                .Replace("!", "")
                .Replace("?", "")
                .Replace("'", "")
                .Replace("\"", "")
                .Replace(":", "")
                .Replace(";", "")
                .Trim();
        }
    }
}