namespace miniprojekt.Model
{
    public class User
    {
        public long UserId { get; set; }
        public string UserName { get; set; }

        // Konstruktør
        public User(string userName)
        {
            UserName = userName;
        }
    }
}