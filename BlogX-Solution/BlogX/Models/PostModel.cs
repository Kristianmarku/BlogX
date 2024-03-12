using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlogX.Data;

public class PostModel
{
    [Key]
    public int PostId { get; set; }

    [Required(ErrorMessage ="This field is required")]
    [Column(TypeName = "nvarchar(100)")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "This field is required")]
    [Column(TypeName = "nvarchar(100)")]
    public string? Content { get; set; }

    [Required(ErrorMessage = "This field is required")]
    [DisplayName("Category")]
    public int CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    public CategoryModel? Category { get; set; }

    [DisplayName("User")]
    public string? UserId { get; set; }

    [ForeignKey("UserId")]
    public UserModel? User { get; set; }

    [NotMapped] // This attribute indicates that it should not be mapped to the database
    [DisplayName("Image")]
    public IFormFile? ImageFile { get; set; }

    [Column(TypeName = "nvarchar(255)")]
    public string? ImageUrl { get; set; }

    [DisplayName("Date Created")]
    [DisplayFormat(DataFormatString = "{0:MMM-dd-yy HH:mm}")]
    public DateTime DateCreated { get; set; }

    [DisplayName("Date Updated")]
    [DisplayFormat(DataFormatString = "{0:MMM-dd-yy HH:mm}")]
    public DateTime DateModified { get; set; }

    // Navigation property for many-to-many relationship with comments
    public ICollection<CommentModel>? Comments { get; set; }
}