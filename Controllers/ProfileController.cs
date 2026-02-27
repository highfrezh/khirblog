using BlogApp.Models;
using BlogApp.Repositories.Interfaces;
using BlogApp.Services.Interfaces;
using BlogApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    [Authorize]
    public class ProfileController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(
            IUserRepository userRepository,
            IBlogPostRepository blogPostRepository,
            IAuthService authService,
            UserManager<ApplicationUser> userManager,
            ICategoryRepository categoryRepository)
            : base(categoryRepository)
        {
            _userRepository = userRepository;
            _blogPostRepository = blogPostRepository;
            _authService = authService;
            _userManager = userManager;
        }

        // GET: /Profile/Index
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var posts = await _blogPostRepository.GetByAuthorAsync(currentUser.Id);
            var postList = posts.ToList();

            var viewModel = new ProfileViewModel
            {
                Id = currentUser.Id,
                FirstName = currentUser.FirstName,
                LastName = currentUser.LastName,
                Email = currentUser.Email!,
                TotalPosts = postList.Count,
                TotalLikes = postList.Sum(p => p.Likes.Count),
                TotalComments = postList.Sum(p => p.Comments.Count),
                RecentPosts = postList
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
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
                    .ToList()
            };

            return View(viewModel);
        }

        // GET: /Profile/Edit
        public async Task<IActionResult> Edit()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var viewModel = new EditProfileViewModel
            {
                Id = currentUser.Id,
                FirstName = currentUser.FirstName,
                LastName = currentUser.LastName,
                Email = currentUser.Email!
            };

            return View(viewModel);
        }

        // POST: /Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email;

            await _userRepository.UpdateAsync(user);

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Index");
        }

        // GET: /Profile/ChangePassword
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        // POST: /Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var success = await _authService.ChangePasswordAsync(
                currentUser.Id,
                model.CurrentPassword,
                model.NewPassword);

            if (success)
            {
                TempData["Success"] = "Password changed successfully!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, "Current password is incorrect.");
            return View(model);
        }
    }
}