using IIS_SERVER.Post.Models;
using Microsoft.AspNetCore.Mvc;

namespace IIS_SERVER.Post.Controllers;

public interface IPostController
{
    Task<IActionResult> GetPost(Guid postId);

    Task<IActionResult> GetPosts(Guid threadId);

    Task<IActionResult> GetPosts(string userEmail);

    Task<IActionResult> AddPost(PostModel post);

    Task<IActionResult> EditPostText(Guid postId, string text);

    Task<IActionResult> EditPost(PostModel post);

    Task<IActionResult> DeletePost(PostModel post);
}
