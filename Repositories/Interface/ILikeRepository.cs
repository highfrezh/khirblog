using BlogApp.Models;

namespace BlogApp.Repositories.Interfaces
{
    public interface ILikeRepository
    {
        Task<IEnumerable<Like>> GetByPostIdAsync(int blogPostId);
        Task<Like?> GetByUserAndPostAsync(string userId, int blogPostId);
        Task<int> GetLikeCountAsync(int blogPostId);
        Task<bool> HasUserLikedAsync(string userId, int blogPostId);
        Task AddAsync(Like like);
        Task DeleteAsync(int id);
    }
}