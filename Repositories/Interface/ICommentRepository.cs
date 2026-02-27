using BlogApp.Models;

namespace BlogApp.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetByPostIdAsync(int blogPostId);
        Task<Comment?> GetByIdAsync(int id);
        Task AddAsync(Comment comment);
        Task UpdateAsync(Comment comment);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}