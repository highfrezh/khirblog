using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApp.Models
{
    public class BlogPostTag
    {
        public int BlogPostId { get; set; }
        public int TagId { get; set; }

        [ForeignKey("BlogPostId")]
        public virtual BlogPost BlogPost { get; set; }

        [ForeignKey("TagId")]
        public virtual Tag Tag { get; set; }
    }
}