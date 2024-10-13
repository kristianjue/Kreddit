namespace shared.Model
{
    public class User
    {
        public long UserId { get; set; }
        public string UserName { get; set; }

        // Default constructor
        public User()
        {
            UserName = string.Empty;
        }
        // Constructor with parameters
        public User(string userName)
        {
            UserName = userName;
        }
    }
}