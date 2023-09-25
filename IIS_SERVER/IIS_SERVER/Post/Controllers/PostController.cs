using IIS_SERVER.Services;
using IIS_SERVER.Post.Models;
using Microsoft.AspNetCore.Mvc;

namespace IIS_SERVER.Post.Controllers;

[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase, IPostController
{
    private readonly IMySQLService MySqlService;

    public PostController(IMySQLService mySqlService)
    {
        MySqlService = mySqlService;
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
    public async Task<IActionResult> GetPosts(Guid threadId)
    {
        var posts = await MySqlService.GetPostsByThread(threadId);
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
        var result = await MySqlService.AddPost(post);
        if (result.Item1)
        {
            return Ok("Post added successfully");
        }
        else
        {
            return BadRequest(result.Item2);
        }
    }

    [HttpPut("updateText")]
    public async Task<IActionResult> EditPostText(Guid postId, string text)
    {
        var result = await MySqlService.EditPostText(postId, text);
        if (result.Item1)
        {
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

    [HttpDelete("delete")]
    public async Task<IActionResult> DeletePost(PostModel post)
    {
        var result = await MySqlService.DeletePost(post);
        if (result.Item1)
        {
            return Ok("Post deleted successfully");
        }
        else
        {
            return BadRequest(result.Item2);
        }
    }
}
