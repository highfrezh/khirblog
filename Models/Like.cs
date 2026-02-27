using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApp.Models
{
    public class Like
    {
        public int Id { get; set; }

        public int BlogPostId { get; set; }

        public string UserId { get; set; }

        [ForeignKey("BlogPostId")]
        public virtual BlogPost BlogPost { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}