
namespace shared.Model
{
    public class Comment
    {
        public long CommentId { get; set; }
        public string Content { get; set; }
        public int Upvotes { get; set; }
        
        public int Downvotes { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
        public int VoteCount { get; set; }

        // Default constructor
        public Comment()
        {
            Content = string.Empty; 
            User = new User(); 
            Date = DateTime.Now;
            Upvotes = 0;
            Downvotes = 0;
        }
        // Constructor with parameters
        public Comment(string content, User user)
        {
            Content = content;
            User = user;
            Date = DateTime.Now;
            Upvotes = 0;
            Downvotes = 0;
        }
        
    }
}