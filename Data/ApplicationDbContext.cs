using BlogApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Core entities
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Like> Likes { get; set; }

        // Join table
        public DbSet<BlogPostTag> BlogPostTags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // BlogPostTag (many-to-many)
            builder.Entity<BlogPostTag>()
                .HasKey(bpt => new { bpt.BlogPostId, bpt.TagId });

            builder.Entity<BlogPostTag>()
                .HasOne(bpt => bpt.BlogPost)
                .WithMany(bp => bp.BlogPostTags)
                .HasForeignKey(bpt => bpt.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BlogPostTag>()
                .HasOne(bpt => bpt.Tag)
                .WithMany(t => t.BlogPostTags)
                .HasForeignKey(bpt => bpt.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            // BlogPost → Category (many posts, one category)
            builder.Entity<BlogPost>()
                .HasOne(bp => bp.Category)
                .WithMany(c => c.BlogPosts)
                .HasForeignKey(bp => bp.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Comment → BlogPost
            builder.Entity<Comment>()
                .HasOne(c => c.BlogPost)
                .WithMany(bp => bp.Comments)
                .HasForeignKey(c => c.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Comment → User
            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Like (unique per user per post)
            builder.Entity<Like>()
                .HasIndex(l => new { l.BlogPostId, l.UserId })
                .IsUnique();

            builder.Entity<Like>()
                .HasOne(l => l.BlogPost)
                .WithMany(bp => bp.Likes)
                .HasForeignKey(l => l.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
