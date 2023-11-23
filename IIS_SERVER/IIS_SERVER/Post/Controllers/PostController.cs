using IIS_SERVER.Services;
using IIS_SERVER.Post.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using IIS_SERVER.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using IIS_SERVER.Thread.Models;
using IIS_SERVER.Helpers;
using IIS_SERVER.Enums;
using IIS_SERVER.Group.Models;

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

    [HttpGet("getGroupHandleByPostId/{postId}")]
    public async Task<IActionResult> GetGroupHandleByPostId(Guid postId)
    {
        GroupListModel group;
        try
        {
            group = await PostHelper.GetGroupByPostOrThread(postId, MySqlService);
        }
        catch
        {
            return NotFound($"No group found");
        }

        return Ok(group.Handle);
    }

    [HttpPost("add")]
    [Authorize(Policy = "AdminUserPolicy")]
    public async Task<IActionResult> AddPost(PostModel post)
    {
        post.Id = Guid.NewGuid();
        try
        {
            bool isMember = await PostHelper.IsPosterInGroup(
                post.ThreadId,
                User.FindFirst(ClaimTypes.Email).Value,
                MySqlService
            );
            if (isMember || User.IsInRole("admin"))
            {
                var result = await MySqlService.AddPost(post);
                if (result.Item1)
                {
                    await hub.Clients
                        .Groups(post.ThreadId.ToString())
                        .SendAsync("NewPost", post.Id);
                    return Ok("Post added successfully");
                }
                else
                {
                    return BadRequest(result.Item2);
                }
            }
            else
            {
                return Forbid();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest();
        }
    }

    [HttpPut("updateText")]
    [Authorize(Policy = "AdminUserPolicy")]
    public async Task<IActionResult> EditPostText(Guid postId, string text)
    {
        bool isOwner;
        try
        {
            isOwner = await PostHelper.IsUserPostOwner(
                postId,
                User.FindFirst(ClaimTypes.Email).Value,
                MySqlService
            );
        }
        catch
        {
            return BadRequest();
        }

        if (isOwner)
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
        else
        {
            return Forbid();
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
        bool isOwner;
        GroupRole memberRole;
        try
        {
            string email = User.FindFirst(ClaimTypes.Email).Value;
            isOwner = await PostHelper.IsUserPostOwner(postId, email, MySqlService);
            memberRole = await PostHelper.getMemberGroupRole(postId, email, MySqlService);
        }
        catch
        {
            return BadRequest();
        }
        if (
            isOwner
            || User.IsInRole("admin")
            || memberRole == GroupRole.admin
            || memberRole == GroupRole.moderator
        )
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
        else
        {
            return Forbid();
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
