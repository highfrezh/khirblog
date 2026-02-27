using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(100)]
        public string? Description { get; set; }

        // Navigation property
        public virtual ICollection<BlogPostTag> BlogPostTags { get; set; } = new List<BlogPostTag>();
    }
}