using BlogApp.Models;
using BlogApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(
            ICommentRepository commentRepository,
            IBlogPostRepository blogPostRepository,
            UserManager<ApplicationUser> userManager)
        {
            _commentRepository = commentRepository;
            _blogPostRepository = blogPostRepository;
            _userManager = userManager;
        }

        // POST: /Comment/Add
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int blogPostId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Comment cannot be empty.";
                return await RedirectToPost(blogPostId);
            }

            var post = await _blogPostRepository.GetByIdAsync(blogPostId);
            if (post == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);

            var comment = new Comment
            {
                Content = content,
                BlogPostId = blogPostId,
                UserId = currentUser!.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _commentRepository.AddAsync(comment);

            TempData["Success"] = "Comment added successfully!";
            return await RedirectToPost(blogPostId);
        }

        // POST: /Comment/Delete
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int blogPostId)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null) return NotFound();

            // Only comment author or admin can delete
            var currentUserId = _userManager.GetUserId(User);
            if (comment.UserId != currentUserId && !User.IsInRole("Admin"))
                return Forbid();

            await _commentRepository.DeleteAsync(id);

            TempData["Success"] = "Comment deleted.";
            return await RedirectToPost(blogPostId);
        }

        // POST: /Comment/Edit
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int blogPostId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Comment cannot be empty.";
                return await RedirectToPost(blogPostId);
            }

            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null) return NotFound();

            // Only comment author can edit
            var currentUserId = _userManager.GetUserId(User);
            if (comment.UserId != currentUserId)
                return Forbid();

            comment.Content = content;
            await _commentRepository.UpdateAsync(comment);

            TempData["Success"] = "Comment updated.";
            return await RedirectToPost(blogPostId);
        }

        // Helper: redirect back to the post detail page
        private async Task<IActionResult> RedirectToPost(int blogPostId)
        {
            var post = await _blogPostRepository.GetByIdAsync(blogPostId);
            if (post == null) return NotFound();

            return RedirectToAction("Detail", "BlogPost", new { slug = post.Slug });
        }
    }
}