using Microsoft.EntityFrameworkCore;
using System.Text.Json;

using Api.Data;
using shared.Model;
using shared.DTO;
using Api.Service;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Sætter CORS så API'en kan bruges fra andre domæner
var AllowSomeStuff = "_AllowSomeStuff";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowSomeStuff, builder => {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Tilføj DbContext factory som service.
builder.Services.AddDbContext<ThreadContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ContextSQLite")));

// Tilføj DataService så den kan bruges i endpoints
builder.Services.AddScoped<DataService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dataService = scope.ServiceProvider.GetRequiredService<DataService>();
    dataService.SeedData(); // Fylder data på, hvis databasen er tom. Ellers ikke.
}

app.UseHttpsRedirection();
app.UseCors(AllowSomeStuff);


// Middlware der kører før hver request. Sætter ContentType for alle responses til "JSON".
app.Use(async (context, next) =>
{
    context.Response.ContentType = "application/json; charset=utf-8";
    await next(context);
});

app.MapGet("/api/posts", (DataService dataService) =>
{
    return dataService.getAllThreads();
});

app.MapGet("/api/posts/{id}", (DataService dataService, long id) =>
{
    return dataService.getThreadById(id);
});

app.MapPut("/api/posts/{id}/upvote", (DataService dataService, long id) =>
{
    try
    {
        var updatedThread = dataService.upVote(id);
        if (updatedThread == null)
        {
            return Results.NotFound();
        }
        return Results.Ok(updatedThread);
    }
    catch (Exception ex)
    {
        // Log the exception
        return Results.Problem($"An error occurred while downvoting: {ex.Message}");
    }
});

// Downvote a post
app.MapPut("/api/posts/{id}/downvote", (DataService dataService, long id) =>
{
    try
    {
        var updatedThread = dataService.downVote(id);
        if (updatedThread == null)
        {
            return Results.NotFound();
        }
        return Results.Ok(updatedThread);
    }
    catch (Exception ex)
    {
        // Log the exception
        return Results.Problem($"An error occurred while downvoting: {ex.Message}");
    }
});

// Upvote a comment
app.MapPut("/api/posts/{id}/comments/{commentid}/upvote", (DataService dataService, long id, long commentid) =>
{
    try
    {
        dataService.upVoteComment(id, commentid);
        var updatedThread = dataService.getThreadById(id); // Return the updated thread with comments
        if (updatedThread == null)
        {
            return Results.NotFound();
        }
        return Results.Ok(updatedThread);
    }
    catch (Exception ex)
    {
        // Log the exception
        return Results.Problem($"An error occurred while upvoting the comment: {ex.Message}");
    }
});

// Downvote a comment
app.MapPut("/api/posts/{id}/comments/{commentid}/downvote", (DataService dataService, long id, long commentid) =>
{
    try
    {
        dataService.downVoteComment(id, commentid);
        var updatedThread = dataService.getThreadById(id); // Return the updated thread with comments
        if (updatedThread == null)
        {
            return Results.NotFound();
        }
        return Results.Ok(updatedThread);
    }
    catch (Exception ex)
    {
        // Log the exception
        return Results.Problem($"An error occurred while downvoting the comment: {ex.Message}");
    }
});

app.MapPost("/api/posts", (DataService dataService, Threads thread) =>
{
    dataService.addThread(thread);
    return Results.Ok();
});

app.MapPost("/api/posts/{id}/comments", (DataService dataService, long id, CommentRequest request) =>
{
    // Opret en ny bruger baseret på request.UserName
    var user = new User { UserName = request.UserName };

    // Hent kun de nødvendige felter fra request.Comment (content) og tilføj brugeren
    var comment = new Comment
    {
        Content = request.Comment.Content,
        User = user // Tildel den oprettede bruger til kommentaren
    };

    // Tilføj kommentaren til tråden ved hjælp af DataService
    dataService.AddComment(comment, user, id);

    return Results.Ok(comment); // Returner den nye kommentar
});




app.Run();
