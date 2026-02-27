using BlogApp.Models;

namespace BlogApp.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task UpdateAsync(ApplicationUser user);
        Task DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
    }
}