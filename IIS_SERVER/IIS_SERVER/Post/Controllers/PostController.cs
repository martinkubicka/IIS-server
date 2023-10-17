using IIS_SERVER.Services;
using IIS_SERVER.Post.Models;
using Microsoft.AspNetCore.Mvc;
using IIS_SERVER.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace IIS_SERVER.Post.Controllers;

[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase, IPostController
{
    private readonly IMySQLService MySqlService;
    private readonly IHubContext<ThreadHub> hub;

    public PostController(IMySQLService mySqlService, IHubContext<ThreadHub> threadHub)
    {
        MySqlService = mySqlService;
        hub = threadHub;
    }

    [HttpGet("getPost/{postId}")]
    public async Task<IActionResult> GetPost(Guid postId)
    {
        var post = await MySqlService.GetPost(postId);
        if (post != null)
        {
            return Ok(post);
        }
        else
        {
            return NotFound($"Post with ID {postId} not found.");
        }
    }

    [HttpGet("getPostsByThread/{threadId}")]
    public async Task<IActionResult> GetPosts(Guid threadId, int limit, int offset)
    {
        var posts = await MySqlService.GetPostsByThread(threadId, limit, offset);
        if (posts != null)
        {
            return Ok(posts);
        }
        else
        {
            return NotFound($"No posts found for thread with ID {threadId}.");
        }
    }

    [HttpGet("getPostsByUser/{userEmail}")]
    public async Task<IActionResult> GetPosts(string userEmail)
    {
        var posts = await MySqlService.GetPostsByUser(userEmail);
        if (posts != null)
        {
            return Ok(posts);
        }
        else
        {
            return NotFound($"No posts found for user with email {userEmail}.");
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddPost(PostModel post)
    {
        post.Id = Guid.NewGuid();
        try
        {
            var result = await MySqlService.AddPost(post);
            if (result.Item1)
            {
                await hub.Clients.Groups(post.ThreadId).SendAsync("NewPost", post.Id);
                return Ok("Post added successfully");
            }
            else
            {
                return BadRequest(result.Item2);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest();
        }
    }

    [HttpPut("updateText")]
    public async Task<IActionResult> EditPostText(Guid postId, string text)
    {
        var result = await MySqlService.EditPostText(postId, text);
        if (result.Item1)
        {
            await hub.Clients.All.SendAsync("UpdatePost", postId, text);
            return Ok("Post text edited successfully");
        }
        else
        {
            return BadRequest(result.Item2);
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> EditPost(PostModel post)
    {
        var result = await MySqlService.EditPost(post);
        if (result.Item1)
        {
            return Ok("Post edited successfully");
        }
        else
        {
            return BadRequest(result.Item2);
        }
    }

    [HttpDelete("delete/{postId}")]
    public async Task<IActionResult> DeletePost(Guid postId)
    {
        var result = await MySqlService.DeletePost(postId);
        if (result.Item1)
        {
            await hub.Clients.All.SendAsync("DeletePost", postId);
            return Ok("Post deleted successfully");
        }
        else
        {
            return BadRequest(result.Item2);
        }
    }

    [HttpGet("calculateRating/{postId}")]
    public async Task<IActionResult> CalculateRating(Guid postId)
    {
        int? count = await MySqlService.CalculateRating(postId);
        if (count != null)
        {
            return Ok(count);
        }
        else
        {
            return NotFound($"Could not calculate rating");
        }
    }
}
