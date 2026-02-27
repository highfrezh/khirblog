using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace BlogApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string? FirstName { get; set; }

        [PersonalData]
        public string? LastName { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsAdmin { get; set; } = false;

        // Navigation properties
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}