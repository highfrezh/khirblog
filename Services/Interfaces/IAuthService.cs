using BlogApp.Models;
using BlogApp.ViewModels;

namespace BlogApp.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string[] Errors)> RegisterAsync(RegisterViewModel model);
        Task<bool> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    }
}