using Microsoft.EntityFrameworkCore;
using Api.Data;
using shared.Model;
using shared.DTO;
using Api.Service;

var builder = WebApplication.CreateBuilder(args);

// Set up CORS to allow the API to be used from other domains
var allowSomeStuff = "_allowSomeStuff";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowSomeStuff, policybuilder => {
        policybuilder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add DbContext factory as a service
builder.Services.AddDbContext<ThreadContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ContextSQLite")));

// Add DataService so it can be used in endpoints
builder.Services.AddScoped<DataService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dataService = scope.ServiceProvider.GetRequiredService<DataService>();
    dataService.SeedData(); // Populate data if the database is empty. Otherwise, do nothing.
}

app.UseHttpsRedirection();
app.UseCors(allowSomeStuff);

// Middleware that runs before each request. Sets ContentType for all responses to "JSON".
app.Use(async (context, next) =>
{
    context.Response.ContentType = "application/json; charset=utf-8";
    await next(context);
});

// GET: Retrieve all posts
app.MapGet("/api/posts", (DataService dataService) =>
{
    return dataService.GetAllThreads();
});

// GET: Retrieve a specific post by ID
app.MapGet("/api/posts/{id}", (DataService dataService, long id) =>
{
    return dataService.GetThreadById(id);
});

// PUT: Upvote a post
app.MapPut("/api/posts/{id}/upvote", (DataService dataService, long id) =>
{
    var updatedThread = dataService.UpVote(id);
    if (updatedThread == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(updatedThread);
});

// PUT: Downvote a post
app.MapPut("/api/posts/{id}/downvote", (DataService dataService, long id) =>
{
    var updatedThread = dataService.DownVote(id);
    if (updatedThread == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(updatedThread);
});

// PUT: Upvote a comment
app.MapPut("/api/posts/{id}/comments/{commentid}/upvote", (DataService dataService, long id, long commentid) =>
{
    bool success = dataService.UpVoteComment(id, commentid);
    if (!success)
    {
        return Results.NotFound();
    }
    var updatedThread = dataService.GetThreadById(id); // Return the updated thread with comments
    return Results.Ok(updatedThread);
});

// PUT: Downvote a comment
app.MapPut("/api/posts/{id}/comments/{commentid}/downvote", (DataService dataService, long id, long commentid) =>
{
    bool success = dataService.DownVoteComment(id, commentid);
    if (!success)
    {
        return Results.NotFound();
    }
    var updatedThread = dataService.GetThreadById(id); // Return the updated thread with comments 
    return Results.Ok(updatedThread);
});

// POST: Create a new post
app.MapPost("/api/posts", (DataService dataService, ThreadRequest request) =>
{
    var newThread = dataService.AddThread(request.Thread.Title, request.Thread.Content, request.UserName);
    return Results.Ok(newThread);
});

// POST: Add a comment to a post
app.MapPost("/api/posts/{id}/comments", (DataService dataService, long id, CommentRequest request) =>
{
    // Create a new user based on request.UserName
    var user = new User { UserName = request.UserName };

    // Get only the necessary fields from request.Comment (content) and add the user
    var comment = new Comment
    {
        Content = request.Comment.Content,
        User = user // Assign the created user to the comment
    };

    // Add the comment to the thread using DataService
    dataService.AddComment(comment, user, id);

    return Results.Ok(comment); // Return the new comment
});

app.Run();