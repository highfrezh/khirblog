using System.ComponentModel.DataAnnotations;

public class TagFormViewModel
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
}
