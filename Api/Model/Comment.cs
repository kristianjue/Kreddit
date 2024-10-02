namespace miniprojekt.Model
{
    public class Comment
    {
        public long CommentId { get; set; }
        public string Content { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
        public int VoteCount { get; set; }

        // Konstruktør
        public Comment(string content, User user)
        {
            Content = content;
            User = user;
            Date = DateTime.Now;
            VoteCount = 0;
        }
        
        public Comment(string content)
        {
            Content = content;
            Date = DateTime.Now;
            VoteCount = 0;
        }
    }
}