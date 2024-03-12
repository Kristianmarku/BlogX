using System;
using Microsoft.AspNetCore.Identity;


public class UserModel : IdentityUser
{
	[PersonalData]
	public string? FirstName { get; set; }

    [PersonalData]
    public string? LastName { get; set; }

    // Read-only property to concatenate FirstName and LastName
    public string FullName => $"{FirstName} {LastName}";

    // Navigation property for many-to-many relationship with posts
    public ICollection<PostModel>? Posts { get; set; }

    // Navigation property for many-to-many relationship with comments
    public ICollection<CommentModel>? Comments { get; set; }

    // Navigation property for many-to-many relationship with roles
    public ICollection<IdentityUserRole<string>> Roles { get; set; } = new List<IdentityUserRole<string>>();
}
