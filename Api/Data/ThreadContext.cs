using shared.Model;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class ThreadContext : DbContext
{
    public DbSet<Threads> Threads { get; set; }
    
  
    public ThreadContext (DbContextOptions<ThreadContext> options)
        : base(options)
    {
        // Den her er tom. Men ": base(options)" sikre at constructor
        // på DbContext super-klassen bliver kaldt.
    }
}