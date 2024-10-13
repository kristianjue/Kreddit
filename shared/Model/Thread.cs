
namespace shared.Model
{
    public class Threads
    {
        public long ThreadsId { get; set; }
        public string Title { get; set; }
        public int Upvotes { get; set; }
        
        public int Downvotes { get; set; }
        public User User { get; set; }
        public List<Comment> Comments { get; set; }
        
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int VoteCount { get; set; }

        // Default constructor
        public Threads()
        {
            Title = string.Empty;
            Content = string.Empty; 
            User = new User();
            Comments = new List<Comment>();
            Date = DateTime.Now;
            Upvotes = 0;
            Downvotes = 0;
        }
        // Constructor with parameters
        public Threads(string title, string content, User user)
        {
            Title = title;
            Content = content;
            User = user;
            Comments = new List<Comment>();
            Date = DateTime.Now;
            Upvotes = 0;
            Downvotes = 0;
        }
       
    }
}