using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
    dataService.upVote(id);
    return Results.Ok();
});

app.MapPut("/api/posts/{id}/downvote", (DataService dataService, long id) =>
{
    dataService.downVote(id);
    return Results.Ok();
});
app.MapPut("/api/posts/{id}/comments/{commentid}/upvote", (DataService dataService, long id, long commentid) =>
{
    dataService.upVoteComment(id, commentid);
    return Results.Ok();
});
app.MapPut("/api/posts/{id}/comments/{commentid}/downvote", (DataService dataService, long id, long commentid) =>
{
    dataService.downVoteComment(id, commentid);
    return Results.Ok();
});
app.MapPost("/api/posts", (DataService dataService, Threads thread) =>
{
    dataService.addThread(thread);
    return Results.Ok();
});
app.MapPost("/api/posts/{id}/comments", (DataService dataService, long id, Comment comment) =>
{
    dataService.addComment(comment, id);
    return Results.Ok();
});

app.Run();