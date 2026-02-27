using BlogApp.Models;
using BlogApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    public class LikeController : BaseController  // ← change this
    {
        private readonly ILikeRepository _likeRepository;
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public LikeController(
            ILikeRepository likeRepository,
            IBlogPostRepository blogPostRepository,
            UserManager<ApplicationUser> userManager,
            ICategoryRepository categoryRepository)  // ← add this
            : base(categoryRepository)               // ← add this
        {
            _likeRepository = likeRepository;
            _blogPostRepository = blogPostRepository;
            _userManager = userManager;
        }

        // POST: /Like/Toggle
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int blogPostId)
        {
            var post = await _blogPostRepository.GetByIdAsync(blogPostId);
            if (post == null) return NotFound();
            var currentUserId = _userManager.GetUserId(User);
            var existingLike = await _likeRepository.GetByUserAndPostAsync(currentUserId!, blogPostId);
            if (existingLike != null)
            {
                await _likeRepository.DeleteAsync(existingLike.Id);
            }
            else
            {
                var like = new Like
                {
                    BlogPostId = blogPostId,
                    UserId = currentUserId!
                };
                await _likeRepository.AddAsync(like);
            }
            return RedirectToAction("Detail", "BlogPost", new { slug = post.Slug });
        }
    }
}