using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using shared.DTO;
using shared.Model;

namespace kreddit_app.Data;

public class ApiService
{
    private readonly HttpClient http;
    private readonly IConfiguration configuration;
    private readonly string baseAPI = "";

    public ApiService(HttpClient http, IConfiguration configuration)
    {
        this.http = http;
        this.configuration = configuration;
        this.baseAPI = configuration["base_api"];
    }

    public async Task<Threads[]> GetPosts()
    {
        string url = $"{baseAPI}posts/";
        return await http.GetFromJsonAsync<Threads[]>(url);
    }

    public async Task<Threads> GetPost(long id)
    {
        string url = $"{baseAPI}posts/{id}";
        return await http.GetFromJsonAsync<Threads>(url);
    }

    public async Task<Comment> CreateComment(String content, string username, int postId)
    {
        string url = $"{baseAPI}posts/{postId}/comments";
    
        // Create an anonymous object to send to the API
        var commentData = new CommentRequest
        {
            UserName = username,
            Comment = new Comment { Content = content }
        };

        await http.PostAsJsonAsync($"api/posts/{postId}/comments", commentData);
    
        // Make a POST request to the API with the comment data
        HttpResponseMessage msg = await http.PostAsJsonAsync(url, commentData);
    
        // Ensure the response is successful
        msg.EnsureSuccessStatusCode();
    
        // Read the response content
        string json = await msg.Content.ReadAsStringAsync();
    
        // Deserialize the response into a Comment object
        Comment newComment = JsonSerializer.Deserialize<Comment>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return newComment; // Return the created comment
    }


    

    public async Task<Threads> UpvotePost(long id)
    {
        string url = $"{baseAPI}posts/{id}/upvote/";

        // Post JSON to API, save the HttpResponseMessage
        HttpResponseMessage msg = await http.PutAsJsonAsync(url, "");

        // Get the JSON string from the response
        string json = msg.Content.ReadAsStringAsync().Result;

        // Deserialize the JSON string to a Post object
        Threads? updatedPost = JsonSerializer.Deserialize<Threads>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true // Ignore case when matching JSON properties to C# properties
        });

        // Return the updated post (vote increased)
        return updatedPost;
    }
    public async Task<Threads> DownvotePost(long id)
    {
        string url = $"{baseAPI}posts/{id}/downvote/";

        // Put request to API, save the HttpResponseMessage
        HttpResponseMessage msg = await http.PutAsJsonAsync(url, "");

        // Get the JSON string from the response
        string json = msg.Content.ReadAsStringAsync().Result;

        // Deserialize the JSON string to a Threads object
        Threads? updatedPost = JsonSerializer.Deserialize<Threads>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true // Ignore case when matching JSON properties to C# properties
        });

        // Return the updated post (vote decreased)
        return updatedPost;
    }
    public async Task<Comment> UpvoteComment(long postId,long commentId)
    {
        string url = $"{baseAPI}posts/{postId}/comments/{commentId}/upvote/";

        // Put request to API, save the HttpResponseMessage
        HttpResponseMessage msg = await http.PutAsJsonAsync(url, "");

        // Get the JSON string from the response
        string json = msg.Content.ReadAsStringAsync().Result;

        // Deserialize the JSON string to a Comment object
        Comment? updatedComment = JsonSerializer.Deserialize<Comment>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true // Ignore case when matching JSON properties to C# properties
        });

        // Return the updated comment (vote increased)
        return updatedComment;
    }


    // Downvoter en comment
    public async Task<Comment> DownvoteComment(long postId,long commentId)
    {
        string url = $"{baseAPI}posts/{postId}/comments/{commentId}/downvote/";

        // Put request to API, save the HttpResponseMessage
        HttpResponseMessage msg = await http.PutAsJsonAsync(url, "");

        // Get the JSON string from the response
        string json = msg.Content.ReadAsStringAsync().Result;

        // Deserialize the JSON string to a Comment object
        Comment? updatedComment = JsonSerializer.Deserialize<Comment>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true // Ignore case when matching JSON properties to C# properties
        });

        // Return the updated comment (vote decreased)
        return updatedComment;
    }

}
