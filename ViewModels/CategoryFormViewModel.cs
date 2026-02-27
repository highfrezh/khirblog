using System.ComponentModel.DataAnnotations;

public class CategoryFormViewModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
}
