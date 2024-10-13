using shared.Model;

namespace shared.DTO;

public class ThreadRequest
{
    public string UserName { get; set; }
    
    public Threads Thread { get; set; }
   
    // Default constructor
    public ThreadRequest()
    {
        UserName = string.Empty;  
        Thread = new Threads();   
    }
}