using System.Net.Http.Json;
using System.Text.Json;
using shared.DTO;
using shared.Model;

namespace kreddit_app.Data;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _configuration;
    private readonly string _baseApi;
    
    public ApiService(HttpClient http, IConfiguration configuration)
    {
        this._http = http;
        this._configuration = configuration;
        this._baseApi = configuration["base_api"];
    }

    public async Task<Threads[]> GetPosts()
    {
        string url = $"{_baseApi}posts/";
        var posts = await _http.GetFromJsonAsync<Threads[]>(url);
    
        if (posts == null)
        {
            return Array.Empty<Threads>(); // Return empty array if null
        }

        return posts;
    }

    public async Task<Threads> GetPost(long id)
    {
        string url = $"{_baseApi}posts/{id}";
        var post = await _http.GetFromJsonAsync<Threads>(url);
    
        if (post == null)
        {
            return new Threads(); // Return a new Threads instance if null
        }

        return post;
    }


    public async Task<Comment> CreateComment(String content, string username, int postId)
    {
        string url = $"{_baseApi}posts/{postId}/comments";
    
        // Create an anonymous object to send to the API
        var commentData = new CommentRequest
        {
            UserName = username,
            Comment = new Comment { Content = content }
        };

        await _http.PostAsJsonAsync($"api/posts/{postId}/comments", commentData);
    
        // Make a POST request to the API with the comment data
        HttpResponseMessage msg = await _http.PostAsJsonAsync(url, commentData);
    
        // Ensure the response is successful
        msg.EnsureSuccessStatusCode();
    
        // Read the response content
        string json = await msg.Content.ReadAsStringAsync();
    
        // Deserialize the response into a Comment object
        Comment? newComment = JsonSerializer.Deserialize<Comment>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (newComment == null)
        {
            throw new Exception("Failed to create comment: received null response from API.");
        }

        return newComment; // Return the created comment
    }
    
    public async Task<Threads> UpvotePost(long id)
    {
        string url = $"{_baseApi}posts/{id}/upvote/";

        // Post JSON to API, save the HttpResponseMessage
        HttpResponseMessage msg = await _http.PutAsJsonAsync(url, "");

        // Get the JSON string from the response
        string json = msg.Content.ReadAsStringAsync().Result;

        // Deserialize the JSON string to a Post object
        Threads? updatedPost = JsonSerializer.Deserialize<Threads>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true 
        });
        
        if (updatedPost == null)
        {
            throw new Exception("Failed to upvote post: received null response from API.");
        }
        // Return the updated post (vote increased)
        return updatedPost;
    }
    public async Task<Threads> DownvotePost(long id)
    {
        string url = $"{_baseApi}posts/{id}/downvote/";

        // Put request to API, save the HttpResponseMessage
        HttpResponseMessage msg = await _http.PutAsJsonAsync(url, "");

        // Get the JSON string from the response
        string json = msg.Content.ReadAsStringAsync().Result;

        // Deserialize the JSON string to a Threads object
        Threads? updatedPost = JsonSerializer.Deserialize<Threads>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true 
        });

        if (updatedPost == null)
        {
            throw new Exception("Failed to upvote post: received null response from API.");
        }
        
        // Return the updated post (vote decreased)
        return updatedPost;
    }
    public async Task<Comment> UpvoteComment(long postId,long commentId)
    {
        string url = $"{_baseApi}posts/{postId}/comments/{commentId}/upvote/";

        // Put request to API, save the HttpResponseMessage
        HttpResponseMessage msg = await _http.PutAsJsonAsync(url, "");

        // Get the JSON string from the response
        string json = msg.Content.ReadAsStringAsync().Result;

        // Deserialize the JSON string to a Comment object
        Comment? updatedComment = JsonSerializer.Deserialize<Comment>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true 
        });
        if (updatedComment == null)
        {
            throw new Exception("Failed to upvote post: received null response from API.");
        }
        // Return the updated comment (vote increased)
        return updatedComment;
    }


    // Downvoter en comment
    public async Task<Comment> DownvoteComment(long postId,long commentId)
    {
        string url = $"{_baseApi}posts/{postId}/comments/{commentId}/downvote/";

        // Put request to API, save the HttpResponseMessage
        HttpResponseMessage msg = await _http.PutAsJsonAsync(url, "");

        // Get the JSON string from the response
        string json = msg.Content.ReadAsStringAsync().Result;

        // Deserialize the JSON string to a Comment object
        Comment? updatedComment = JsonSerializer.Deserialize<Comment>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true 
        });
        if (updatedComment == null)
        {
            throw new Exception("Failed to upvote post: received null response from API.");
        }
        // Return the updated comment (vote decreased)
        return updatedComment;
    }
    public async Task<Threads> CreateThread(string title, string content, string username)
    {
        string url = $"{_baseApi}posts";

        var threadData = new ThreadRequest
        {
            UserName = username,
            Thread = new Threads
            {
                Title = title,
                Content = content
            }
        };

        HttpResponseMessage msg = await _http.PostAsJsonAsync(url, threadData);

        msg.EnsureSuccessStatusCode();

        string json = await msg.Content.ReadAsStringAsync();

        Threads? newThread = JsonSerializer.Deserialize<Threads>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        if (newThread == null)
        {
            throw new Exception("Failed to upvote post: received null response from API.");
        }
        return newThread;
    }


}
