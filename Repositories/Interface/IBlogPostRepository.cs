using BlogApp.Models;

namespace BlogApp.Repositories.Interfaces
{
    public interface IBlogPostRepository
    {
        Task<IEnumerable<BlogPost>> GetAllAsync();
        Task<IEnumerable<BlogPost>> GetPublishedAsync();
        Task<BlogPost?> GetByIdAsync(int id);
        Task<BlogPost?> GetBySlugAsync(string slug);
        Task<IEnumerable<BlogPost>> GetByAuthorAsync(string authorId);
        Task<IEnumerable<BlogPost>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<BlogPost>> GetByTagAsync(int tagId);
        Task<IEnumerable<BlogPost>> SearchAsync(string keyword);
        Task AddAsync(BlogPost blogPost);
        Task UpdateAsync(BlogPost blogPost);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> SlugExistsAsync(string slug);
    }
}