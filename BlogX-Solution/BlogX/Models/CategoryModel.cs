using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class CategoryModel
{
    [Key]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "This field is required")]
    [Column(TypeName = "nvarchar(100)")]
    [DisplayName("Category")]
    public string? CategoryName { get; set; }

    // Navigation property for one-to-many relationship with posts
    public ICollection<PostModel>? Posts { get; set; }
}
