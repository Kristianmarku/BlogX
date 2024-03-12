using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BlogX.Models;
using System.Reflection.Emit;

namespace BlogX.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserModel>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CategoryModel>? Categories { get; set; }
        public DbSet<PostModel>? Posts { get; set; }
        public DbSet<CommentModel>? Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserModel>()
                .Property(e => e.FirstName)
                .HasMaxLength(250);

            builder.Entity<UserModel>()
                .Property(e => e.LastName)
                .HasMaxLength(250);

            // Configure the one-to-many relationship between PostModel and CommentModel
            builder.Entity<PostModel>()
                .HasMany(post => post.Comments)
                .WithOne(comment => comment.Post)
                .HasForeignKey(comment => comment.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}