using BlogApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApp.Models
{
    public class BlogPost
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(200)]
        public string Slug { get; set; }

        [Required]
        public string Content { get; set; }

        [StringLength(500)]
        public string? Excerpt { get; set; }

        [StringLength(300)]
        public string? ImageUrl { get; set; }  // Nullable, optional

        [Display(Name = "Created Date")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Updated Date")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Published")]
        public bool IsPublished { get; set; } = true;

        // Foreign key for Author
        public string AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public virtual ApplicationUser Author { get; set; }

        // Foreign key for Category
        public int CategoryId { get; set; }

        // Navigation

        // Navigation properties
        public virtual Category Category { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
        public virtual ICollection<BlogPostTag> BlogPostTags { get; set; } = new List<BlogPostTag>();
    }
}