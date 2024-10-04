using shared.Model;

namespace shared.DTO;

public class CommentRequest
{
    public string UserName { get; set; }
    public Comment Comment { get; set; }
}