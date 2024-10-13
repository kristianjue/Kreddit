using Microsoft.EntityFrameworkCore;
using Api.Data;
using shared.Model;

namespace Api.Service;

public class DataService
{
    private ThreadContext Db { get; }

    public DataService(ThreadContext db) {
        this.Db = db;
    }
   
    public void SeedData()
    {
        Db.Database.EnsureDeleted();
        Db.Database.EnsureCreated();
        var user1 = new User("Rasmus");
        var user2 = new User("Fabian");
        var user3 = new User("Kristian");
        
        
        var thread1 = new Threads("Cykel","Se min fede cykel Kristian" ,user2  );
        var thread2 = new Threads("Hej", "Jeg savner dig", user1);
        var thread3 = new Threads("Bil", "Se min nye bil", user3);
        
        Db.Threads.Add(thread1);
        Db.Threads.Add(thread2);
        Db.Threads.Add(thread3);
        
        Db.SaveChanges();
        
        var comment1 = new Comment("Hvor er den fed", user3);
        var comment2 = new Comment("Jeg savner også dig", user2);
        var comment3 = new Comment("Jeg savner ikke dig", user3);
        var comment4 = new Comment("Jeg vil også have en", user1);
        
        thread1.Comments.Add(comment1);
        thread2.Comments.Add(comment2);
        thread2.Comments.Add(comment3);
        thread3.Comments.Add(comment4);
        
        Db.SaveChanges();
        
    }
    
    public List<Threads> GetAllThreads()
    {
        return Db.Threads.Include(t => t.User).Include(t => t.Comments).ToList();
    }
    
    public Threads GetThreadById(long id)
    {
        var thread = Db.Threads
            .Include(t => t.User)         
            .Include(t => t.Comments)
            .ThenInclude(c => c.User)  
            .FirstOrDefault(t => t.ThreadsId == id);
    
        if (thread == null)
        {
            throw new KeyNotFoundException($"Thread with ID {id} not found.");
        }
    
        return thread;
    }

    public Threads? UpVote(long id)
    {
        var thread = Db.Threads.FirstOrDefault(t => t.ThreadsId == id);
        if (thread == null)
        {
            return null;
        }
        thread.Upvotes++;
        Db.SaveChanges();
        return thread;
    }
    public Threads? DownVote(long id)
    {
        var thread = Db.Threads.FirstOrDefault(t => t.ThreadsId == id);
        if (thread == null)
        {
            return null;
        }
        thread.Downvotes++;
        Db.SaveChanges();
        return thread;
    }
    public bool UpVoteComment(long id, long commentId)
    {
        var thread = Db.Threads.Include(t => t.Comments).FirstOrDefault(t => t.ThreadsId == id);
        if (thread == null)
        {
            return false;
        }
        var comment = thread.Comments.FirstOrDefault(c => c.CommentId == commentId);
        if (comment == null)
        {
            return false;
        }
        comment.Upvotes++;
        Db.SaveChanges();
        return true;
    }
    public bool DownVoteComment(long id, long commentId)
    {
        var thread = Db.Threads.Include(t => t.Comments).FirstOrDefault(t => t.ThreadsId == id);
        if (thread == null)
        {
            return false;
        }
        var comment = thread.Comments.FirstOrDefault(c => c.CommentId == commentId);
        if (comment == null)
        {
            return false;
        }
        comment.Downvotes++;
        Db.SaveChanges();
        return true;
    }
    public Threads AddThread(string title, string content, string username)
    {
        var user = new User(username);

        var thread = new Threads(title, content, user); 
        Db.Threads.Add(thread);
        Db.SaveChanges();
        return thread;
    }


    public void AddComment(Comment comment, User user, long threadId)
    {
        // Retrieve the thread including its comments using the provided threadId
        var thread = Db.Threads.Include(t => t.Comments).FirstOrDefault(t => t.ThreadsId == threadId);

        if (thread != null)
        {
            // Assign the user to the comment
            comment.User = user; 
        
            // Add the comment to the thread's comments
            thread.Comments.Add(comment);
        
            // Save changes to the database
            Db.SaveChanges();
        }
        else
        {
            throw new KeyNotFoundException($"Thread with ID {threadId} not found.");
        }
    }

}