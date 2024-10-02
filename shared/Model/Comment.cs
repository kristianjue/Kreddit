using shared.Model;


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

        public Comment()
        {
            Date = DateTime.Now;
        }
        // Konstruktør
        public Comment(string content, User user)
        {
            Content = content;
            User = user;
            Date = DateTime.Now;
            Upvotes = 0;
            Downvotes = 0;
        }
        
        public Comment(string content)
        {
            Content = content;
            Date = DateTime.Now;
            Upvotes = 0;
            Downvotes = 0;
        }
    }
}