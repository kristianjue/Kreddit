using shared.Model;

namespace shared.DTO;

public class CommentRequest
{
    public string UserName { get; set; }
    public Comment Comment { get; set; }
    
    // Default constructor
    public CommentRequest()
    {
        UserName = string.Empty;  
        Comment = new Comment();   
    }
}