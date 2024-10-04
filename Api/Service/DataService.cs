using Microsoft.EntityFrameworkCore;
using System.Text.Json;

using Api.Data;
using shared.Model;

namespace Api.Service;

public class DataService
{
    private ThreadContext db { get; }

    public DataService(ThreadContext db) {
        this.db = db;
    }
   
    public void SeedData()
    {
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        var User = new User("Rasmus");
        var User2 = new User("Fabian");
        var User3 = new User("Kristian");
        
        
        var thread1 = new Threads("Cykel","Se min fede cykel Kristian" ,User2,0,0  );
        var thread2 = new Threads("Hej", "Jeg savner dig", User,0,0);
        var thread3 = new Threads("Bil", "Se min nye bil", User3,0,0);
        
        db.Threads.Add(thread1);
        db.Threads.Add(thread2);
        db.Threads.Add(thread3);
        
        db.SaveChanges();
        
        var comment1 = new Comment("Hvor er den fed", User3);
        var comment2 = new Comment("Jeg savner også dig", User2);
        var comment3 = new Comment("Jeg savner ikke dig", User3);
        var comment4 = new Comment("Jeg vil også have en", User);
        
        thread1.Comments.Add(comment1);
        thread2.Comments.Add(comment2);
        thread2.Comments.Add(comment3);
        thread3.Comments.Add(comment4);
        
        db.SaveChanges();
        
    }
    
    public List<Threads> getAllThreads()
    {
        return db.Threads.Include(t => t.User).Include(t => t.Comments).ToList();
    }
    
    public Threads getThreadById(long id)
    {
        return db.Threads
            .Include(t => t.User)          // Include User for Threads
            .Include(t => t.Comments)
            .ThenInclude(c => c.User)  // Include User for Comments
            .FirstOrDefault(t => t.ThreadsId == id);
    }

    public Threads upVote(long id)
    {
        var thread = db.Threads.FirstOrDefault(t => t.ThreadsId == id);
        thread.Upvotes++;
        db.SaveChanges();
        return thread;
    }
    public Threads downVote(long id)
    {
        var thread = db.Threads.FirstOrDefault(t => t.ThreadsId == id);
        thread.Downvotes++;
        db.SaveChanges();
        return thread;
    }
    public void upVoteComment(long id, long commentId)
    {
        var thread = db.Threads.Include(t => t.Comments).FirstOrDefault(t => t.ThreadsId == id);
        var comment = thread.Comments.FirstOrDefault(c => c.CommentId == commentId);
        comment.Upvotes++;
        db.SaveChanges();
    }
    public void downVoteComment(long id, long commentId)
    {
        var thread = db.Threads.Include(t => t.Comments).FirstOrDefault(t => t.ThreadsId == id);
        var comment = thread.Comments.FirstOrDefault(c => c.CommentId == commentId);
        comment.Downvotes++;
        db.SaveChanges();
    }
    public void addThread(Threads thread)
    {
        db.Threads.Add(thread);
        db.SaveChanges();
    }


    public void AddComment(Comment comment, long threadId)
    {
        var thread = db.Threads
            .Include(t => t.Comments)
            .FirstOrDefault(t => t.ThreadsId == threadId);

        if (thread != null)
        {
            // Check if the user already exists in the thread's comments
            var existingUser = thread.Comments
                .Select(c => c.User)
                .FirstOrDefault(u => u.UserName == comment.User.UserName);

            if (existingUser == null)
            {
                // If the user doesn't exist in the comments, use the new user
                comment.User = comment.User;
            }
            else
            {
                // If the user exists, use the existing user
                comment.User = existingUser;
            }

            thread.Comments.Add(comment);
            db.SaveChanges();
        }
        else
        {
            throw new KeyNotFoundException($"Thread with ID {threadId} not found.");
        }


    }
}