namespace shared.Model
{
    public class User
    {
        public long UserId { get; set; }
        public string UserName { get; set; }

        public User()
        {
        }
        // Konstruktør
        public User(string userName)
        {
            UserName = userName;
        }
    }
}