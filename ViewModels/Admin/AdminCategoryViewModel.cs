using System.ComponentModel.DataAnnotations;

namespace BlogApp.ViewModels.Admin
{
    public class AdminCategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int PostCount { get; set; }
    }

    public class AdminCategoryFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}