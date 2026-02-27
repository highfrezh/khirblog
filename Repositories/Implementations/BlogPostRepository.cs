using BlogApp.Data;
using BlogApp.Models;
using BlogApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Repositories.Implementations
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly ApplicationDbContext _context;

        public BlogPostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await _context.BlogPosts
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Likes)
                .Include(b => b.BlogPostTags)
                    .ThenInclude(bt => bt.Tag)
                .Include(b => b.Comments)
                    .ThenInclude(c => c.User)  // ← add this
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlogPost>> GetPublishedAsync()
        {
            return await _context.BlogPosts
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Likes)
                .Include(b => b.BlogPostTags)
                    .ThenInclude(bt => bt.Tag)
                .Include(b => b.Comments)
                    .ThenInclude(c => c.User)  // ← add this
                .Where(b => b.IsPublished)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<BlogPost?> GetByIdAsync(int id)
        {
            return await _context.BlogPosts
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Comments)
                    .ThenInclude(c => c.User)
                .Include(b => b.Likes)
                .Include(b => b.BlogPostTags)
                    .ThenInclude(bt => bt.Tag)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<BlogPost?> GetBySlugAsync(string slug)
        {
            return await _context.BlogPosts
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Comments)
                    .ThenInclude(c => c.User)
                .Include(b => b.Likes)
                .Include(b => b.BlogPostTags)
                    .ThenInclude(bt => bt.Tag)
                .FirstOrDefaultAsync(b => b.Slug == slug);
        }

        public async Task<IEnumerable<BlogPost>> GetByAuthorAsync(string authorId)
        {
            return await _context.BlogPosts
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Comments)
                .Include(b => b.Likes)
                .Include(b => b.BlogPostTags)
                    .ThenInclude(bt => bt.Tag)
                .Where(b => b.AuthorId == authorId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlogPost>> GetByCategoryAsync(int categoryId)
        {
            return await _context.BlogPosts
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Comments)
                .Include(b => b.Likes)
                .Include(b => b.BlogPostTags)
                    .ThenInclude(bt => bt.Tag)
                .Where(b => b.CategoryId == categoryId && b.IsPublished)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlogPost>> GetByTagAsync(int tagId)
        {
            return await _context.BlogPosts
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Comments)
                .Include(b => b.Likes)
                .Include(b => b.BlogPostTags)
                    .ThenInclude(bt => bt.Tag)
                .Where(b => b.BlogPostTags.Any(bt => bt.TagId == tagId) && b.IsPublished)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlogPost>> SearchAsync(string keyword)
        {
            return await _context.BlogPosts
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Comments)
                .Include(b => b.Likes)
                .Include(b => b.BlogPostTags)
                    .ThenInclude(bt => bt.Tag)
                .Where(b => b.IsPublished &&
                    (b.Title.Contains(keyword) ||
                     b.Content.Contains(keyword) ||
                     b.Excerpt.Contains(keyword)))
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(BlogPost blogPost)
        {
            await _context.BlogPosts.AddAsync(blogPost);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BlogPost blogPost)
        {
            _context.BlogPosts.Update(blogPost);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost != null)
            {
                _context.BlogPosts.Remove(blogPost);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.BlogPosts.AnyAsync(b => b.Id == id);
        }

        public async Task<bool> SlugExistsAsync(string slug)
        {
            return await _context.BlogPosts.AnyAsync(b => b.Slug == slug);
        }
    }
}