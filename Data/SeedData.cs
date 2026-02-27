using BlogApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            // Ensure database is up to date
            await context.Database.MigrateAsync();

            // Create roles if they don't exist
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create admin user
            string adminEmail = "admin@blogapp.com";
            string adminPassword = "Admin@123";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true,
                    IsAdmin = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Create sample Categories
            if (!context.Categories.Any())
            {
                var categories = new[]
                {
                    new Category { Name = "General",   Description = "General blog posts" },
                    new Category { Name = "Tutorial",  Description = "Step-by-step guides and tutorials" },
                    new Category { Name = "News",      Description = "Latest news and updates" },
                    new Category { Name = "Opinion",   Description = "Opinion pieces and editorials" },
                    new Category { Name = "Review",    Description = "Product and service reviews" }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // Create sample Tags
            if (!context.Tags.Any())
            {
                var tags = new[]
                {
                    new Tag { Name = "Technology",   Description = "Tech related posts" },
                    new Tag { Name = "Programming",  Description = "Programming tutorials and tips" },
                    new Tag { Name = "Lifestyle",    Description = "Lifestyle and personal development" },
                    new Tag { Name = "Travel",       Description = "Travel experiences and guides" },
                    new Tag { Name = "Food",         Description = "Food recipes and reviews" }
                };

                await context.Tags.AddRangeAsync(tags);
                await context.SaveChangesAsync();
            }
        }
    }
}