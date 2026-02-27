using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(300)]
        public string? Description { get; set; }

        // Navigation property
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
    }
}
