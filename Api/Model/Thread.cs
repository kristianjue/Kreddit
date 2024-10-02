using miniprojekt.Model;

namespace miniprojekt.Model
{
    public class Threads
    {
        public long ThreadsId { get; set; }
        public string Title { get; set; }
        public User User { get; set; }
        public List<Comment> Comments { get; set; }
        
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int VoteCount { get; set; }

        // Konstruktør
        public Threads(string title, string content, User user)
        {
            Title = title;
            Content = content;
            User = user;
            Comments = new List<Comment>();
            Date = DateTime.Now;
            VoteCount = 0;
        }
        public Threads(string title, string content)
        {
            Title = title;
            Content = content;
            Comments = new List<Comment>();
            Date = DateTime.Now;
            VoteCount = 0;
        }
    }
}