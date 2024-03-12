using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class CommentModel
{
    [Key]
    public int CommentId { get; set; }

    public int PostId { get; set; }

    [DisplayName("User")]
    public string? UserId { get; set; }

    public string? Content { get; set; }

    [DisplayName("Date Created")]
    [DisplayFormat(DataFormatString = "{0:MMM-dd-yy HH:mm}")]
    public DateTime DateCreated { get; set; }

    [DisplayName("Date Modified")]
    [DisplayFormat(DataFormatString = "{0:MMM-dd-yy HH:mm}")]
    public DateTime DateModified { get; set; }

    [ForeignKey("PostId")]
    public PostModel? Post { get; set; }

    [ForeignKey("UserId")]
    public UserModel? User { get; set; }
}
