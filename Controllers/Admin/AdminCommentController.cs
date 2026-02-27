using BlogApp.Repositories.Interfaces;
using BlogApp.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminCommentController : Controller
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IBlogPostRepository _blogPostRepository;

        public AdminCommentController(
            ICommentRepository commentRepository,
            IBlogPostRepository blogPostRepository)
        {
            _commentRepository = commentRepository;
            _blogPostRepository = blogPostRepository;
        }

        // GET: /AdminComment/Index
        public async Task<IActionResult> Index()
        {
            var posts = await _blogPostRepository.GetAllAsync();

            var comments = posts
                .SelectMany(p => p.Comments.Select(c => new AdminCommentViewModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    // Fix: null check on User
                    AuthorName = c.User != null ? $"{c.User.FirstName} {c.User.LastName}" : "Unknown",
                    PostTitle = p.Title,
                    PostSlug = p.Slug,
                    BlogPostId = p.Id,
                    CreatedAt = c.CreatedAt
                }))
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            return View(comments);
        }

        // POST: /AdminComment/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null) return NotFound();

            await _commentRepository.DeleteAsync(id);

            TempData["Success"] = "Comment deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}