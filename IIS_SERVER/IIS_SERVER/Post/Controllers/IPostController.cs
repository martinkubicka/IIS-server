/**
* @file IPostController.cs
* author { Dominik Petrik (xpetri25) }
* @date 17.12.2023
* @brief Declaration of controller for post endpoints
*/

using IIS_SERVER.Post.Models;
using Microsoft.AspNetCore.Mvc;

namespace IIS_SERVER.Post.Controllers;

public interface IPostController
{
    Task<IActionResult> GetPost(Guid postId);

    Task<IActionResult> GetPosts(Guid threadId, int limit, int offset);

    Task<IActionResult> GetPosts(string userEmail);

    Task<IActionResult> GetGroupHandleByPostId(Guid postId);

    Task<IActionResult> AddPost(PostModel post);

    Task<IActionResult> EditPostText(Guid postId, string text);

    Task<IActionResult> EditPost(PostModel post);

    Task<IActionResult> DeletePost(Guid postId);

    Task<IActionResult> CalculateRating(Guid postId);

    Task<IActionResult> GetPostsGroupedByThread(
        string userHandle,
        int threadLimit = 10,
        int postsPerThreadLimit = 10
    );
}
