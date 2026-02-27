using BlogApp.Models;
using BlogApp.Repositories.Interfaces;
using BlogApp.Services.Interfaces;
using BlogApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogApp.Controllers
{
    public class BlogPostController : BaseController  // ← change Controller to BaseController
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IImageService _imageService;
        private readonly UserManager<ApplicationUser> _userManager;

        public BlogPostController(
            IBlogPostRepository blogPostRepository,
            ICategoryRepository categoryRepository,
            ITagRepository tagRepository,
            IImageService imageService,
            UserManager<ApplicationUser> userManager)
            : base(categoryRepository)  // ← add this
        {
            _blogPostRepository = blogPostRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            _imageService = imageService;
            _userManager = userManager;
        }

        // GET: /BlogPost/Index
        public async Task<IActionResult> Index(string? search, int? categoryId, int? tagId, int page = 1)
        {
            int pageSize = 6;

            var posts = await _blogPostRepository.GetPublishedAsync();

            // Filter by search
            if (!string.IsNullOrEmpty(search))
                posts = await _blogPostRepository.SearchAsync(search);

            // Filter by category
            if (categoryId.HasValue)
                posts = await _blogPostRepository.GetByCategoryAsync(categoryId.Value);

            // Filter by tag
            if (tagId.HasValue)
                posts = await _blogPostRepository.GetByTagAsync(tagId.Value);

            // Pagination
            var totalPosts = posts.Count();
            var pagedPosts = posts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var categories = await _categoryRepository.GetAllAsync();
            var tags = await _tagRepository.GetAllAsync();

            var viewModel = new BlogPostIndexViewModel
            {
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
                }).ToList(),

                Categories = categories.Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    PostCount = c.BlogPosts.Count
                }).ToList(),

                Tags = tags.Select(t => new TagViewModel
                {
                    Id = t.Id,
                    Name = t.Name
                }).ToList(),

                SearchKeyword = search,
                SelectedCategoryId = categoryId,
                SelectedTagId = tagId,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalPosts / (double)pageSize)
            };

            return View(viewModel);
        }

        // GET: /BlogPost/Detail/slug
        public async Task<IActionResult> Detail(string slug)
        {
            var post = await _blogPostRepository.GetBySlugAsync(slug);
            if (post == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);

            var viewModel = new BlogPostDetailViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Slug = post.Slug,
                Content = post.Content,
                Excerpt = post.Excerpt,
                ImageUrl = post.ImageUrl,
                AuthorName = $"{post.Author.FirstName} {post.Author.LastName}",
                AuthorId = post.AuthorId,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                CategoryName = post.Category.Name,
                CategoryId = post.CategoryId,
                Tags = post.BlogPostTags.Select(bt => bt.Tag.Name).ToList(),
                LikeCount = post.Likes.Count,
                HasLiked = post.Likes.Any(l => l.UserId == currentUserId),
                Comments = post.Comments.Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    AuthorName = $"{c.User.FirstName} {c.User.LastName}",
                    AuthorId = c.UserId,
                    CreatedAt = c.CreatedAt
                }).ToList()
            };

            return View(viewModel);
        }

        // GET: /BlogPost/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var tags = await _tagRepository.GetAllAsync();

            var viewModel = new BlogPostFormViewModel
            {
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }),
                Tags = tags.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                })
            };

            return View(viewModel);
        }

        // POST: /BlogPost/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogPostFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            // Generate slug
            var slug = GenerateSlug(model.Title);

            // Make slug unique
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

            // Handle image upload
            if (model.ImageFile != null)
                blogPost.ImageUrl = await _imageService.UploadImageAsync(model.ImageFile);

            await _blogPostRepository.AddAsync(blogPost);

            // Add tags
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

            TempData["Success"] = "Blog post created successfully!";
            return RedirectToAction("Detail", new { slug = blogPost.Slug });
        }

        // GET: /BlogPost/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _blogPostRepository.GetByIdAsync(id);
            if (post == null) return NotFound();

            // Only author or admin can edit
            var currentUserId = _userManager.GetUserId(User);
            if (post.AuthorId != currentUserId && !User.IsInRole("Admin"))
                return Forbid();

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

        // POST: /BlogPost/Edit/5
        [HttpPost]
        [Authorize]
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

            // Only author or admin can edit
            var currentUserId = _userManager.GetUserId(User);
            if (post.AuthorId != currentUserId && !User.IsInRole("Admin"))
                return Forbid();

            post.Title = model.Title;
            post.Content = model.Content;
            post.Excerpt = model.Excerpt;
            post.IsPublished = model.IsPublished;
            post.CategoryId = model.CategoryId;
            post.UpdatedAt = DateTime.UtcNow;

            // Handle image upload
            if (model.ImageFile != null)
            {
                _imageService.DeleteImage(post.ImageUrl);
                post.ImageUrl = await _imageService.UploadImageAsync(model.ImageFile);
            }

            // Update tags
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

            TempData["Success"] = "Blog post updated successfully!";
            return RedirectToAction("Detail", new { slug = post.Slug });
        }

        // POST: /BlogPost/Delete/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _blogPostRepository.GetByIdAsync(id);
            if (post == null) return NotFound();

            // Only author or admin can delete
            var currentUserId = _userManager.GetUserId(User);
            if (post.AuthorId != currentUserId && !User.IsInRole("Admin"))
                return Forbid();

            // Delete image from server
            _imageService.DeleteImage(post.ImageUrl);

            await _blogPostRepository.DeleteAsync(id);

            TempData["Success"] = "Blog post deleted successfully!";
            return RedirectToAction("Index");
        }

        // GET: /BlogPost/MyPosts
        [Authorize]
        public async Task<IActionResult> MyPosts()
        {
            var currentUserId = _userManager.GetUserId(User);
            var posts = await _blogPostRepository.GetByAuthorAsync(currentUserId!);

            var viewModel = posts.Select(p => new BlogPostViewModel
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
            }).ToList();

            return View(viewModel);
        }

        // Private helpers
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