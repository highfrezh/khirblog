using Microsoft.AspNetCore.Http;

namespace BlogApp.Services.Interfaces
{
    public interface IImageService
    {
        Task<string?> UploadImageAsync(IFormFile imageFile);
        void DeleteImage(string? imageUrl);
    }
}