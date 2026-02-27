// Services/ImageService.cs
namespace BlogApp.Services
{
    public class ImageService
    {
        private readonly IWebHostEnvironment _environment;

        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string?> UploadImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(imageFile.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException("Invalid file type. Only jpg, png, gif, webp are allowed.");

            // Validate file size (e.g. max 5MB)
            if (imageFile.Length > 5 * 1024 * 1024)
                throw new InvalidOperationException("File size cannot exceed 5MB.");

            // Create uploads folder if it doesn't exist
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "images");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Generate unique filename
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/uploads/images/{uniqueFileName}";
        }

        public void DeleteImage(string? imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            var filePath = Path.Combine(_environment.WebRootPath, imageUrl.TrimStart('/'));
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}