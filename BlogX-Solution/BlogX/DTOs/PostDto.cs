namespace BlogX.DTOs
{
    public class PostDto
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int CategoryId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public UserDto User { get; set; }
        public List<CommentDto> Comments { get; set; }
    }
}
