using BlogApp.Data;
using BlogApp.Models;
using BlogApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Repositories.Implementations
{
    public class LikeRepository : ILikeRepository
    {
        private readonly ApplicationDbContext _context;

        public LikeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Like>> GetByPostIdAsync(int blogPostId)
        {
            return await _context.Likes
                .Include(l => l.User)
                .Where(l => l.BlogPostId == blogPostId)
                .ToListAsync();
        }

        public async Task<Like?> GetByUserAndPostAsync(string userId, int blogPostId)
        {
            return await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == userId && l.BlogPostId == blogPostId);
        }

        public async Task<int> GetLikeCountAsync(int blogPostId)
        {
            return await _context.Likes
                .CountAsync(l => l.BlogPostId == blogPostId);
        }

        public async Task<bool> HasUserLikedAsync(string userId, int blogPostId)
        {
            return await _context.Likes
                .AnyAsync(l => l.UserId == userId && l.BlogPostId == blogPostId);
        }

        public async Task AddAsync(Like like)
        {
            await _context.Likes.AddAsync(like);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var like = await _context.Likes.FindAsync(id);
            if (like != null)
            {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }
        }
    }
}