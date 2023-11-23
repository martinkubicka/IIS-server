using IIS_SERVER.Services;
using IIS_SERVER.Rating.Models;
using Microsoft.AspNetCore.Mvc;
using IIS_SERVER.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace IIS_SERVER.Rating.Controllers;

[ApiController]
[Route("[controller]")]
public class RatingController : ControllerBase, IPostController
{
    private readonly IMySQLService _mySqlService;

    public RatingController(IMySQLService mySqlService)
    {
        _mySqlService = mySqlService;
    }

    [HttpGet("{ratingId}")]
    public async Task<IActionResult> GetRating(Guid ratingId)
    {
        var rating = await _mySqlService.GetRating(ratingId);
        if (rating != null)
        {
            return Ok(rating);
        }
        else
        {
            return NotFound($"Rating with ID {ratingId} not found.");
        }
    }

    [HttpGet("getRatings/{postId}")]
    public async Task<IActionResult> GetRatings(Guid postId)
    {
        var ratings = await _mySqlService.GetRatings(postId);
        if (ratings != null)
        {
            return Ok(ratings);
        }
        else
        {
            return NotFound($"No ratings found for post with ID {postId}.");
        }
    }

    [HttpGet("getRatingByPostAndUser/{postId}/{userEmail}")]
    public async Task<IActionResult> GetRatingByPostAndUser(Guid postId, string userEmail)
    {
        var rating = await _mySqlService.GetRatingByPost(postId, userEmail);
        if (rating != null)
        {
            return Ok(rating.Rating);
        }
        else
        {
            return Ok(0);
        }
    }

    [HttpPost("update")]
    [Authorize(Policy = "AdminUserPolicy")]
    public async Task<IActionResult> UpdateRating(Guid postId, string userEmail, int ratingChange)
    {
        bool isMember = await PostHelper.IsUserMemberByPost(postId, userEmail, _mySqlService);

        if (isMember || User.IsInRole("admin"))
        {
            var resultGet = await _mySqlService.GetRatingByPost(postId, userEmail);
            if (ratingChange == 0)
            {
                if (resultGet != null)
                {
                    var resultRemove = await _mySqlService.RemoveRating(resultGet.Id);
                    if (resultRemove.Item1)
                    {
                        return Ok("Rating updated successfully");
                    }
                    else
                    {
                        return BadRequest(resultRemove.Item2);
                    }
                }
                return Ok("Rating updated successfully");
            }
            else
            {
                if (resultGet == null)
                {
                    RatingModel newRating = new RatingModel
                    {
                        Id = Guid.NewGuid(),
                        Email = userEmail,
                        PostId = postId,
                        Rating = ratingChange == 1
                    };
                    var result = await _mySqlService.AddRating(newRating);
                    if (result.Item1)
                    {
                        return Ok("Rating updated successfully");
                    }
                    else
                    {
                        return BadRequest(result.Item2);
                    }
                }
                else
                {
                    var result = await _mySqlService.UpdateRating(resultGet.Id, ratingChange);
                    if (result.Item1)
                    {
                        return Ok("Rating updated successfully");
                    }
                    else
                    {
                        return BadRequest(result.Item2);
                    }
                }
            }
        }
        else
        {
            return Forbid();
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddRating(RatingModel rating)
    {
        var result = await _mySqlService.AddRating(rating);
        if (result.Item1)
        {
            return Ok("Rating added successfully");
        }
        else
        {
            return BadRequest(result.Item2);
        }
    }

    [HttpDelete("removeRating/{ratingId}")]
    public async Task<IActionResult> RemoveRating(Guid ratingId)
    {
        var result = await _mySqlService.RemoveRating(ratingId);
        if (result.Item1)
        {
            return Ok("Rating removed successfully");
        }
        else
        {
            return BadRequest(result.Item2);
        }
    }

    [HttpPut("toggleRating/{ratingId}")]
    public async Task<IActionResult> ToggleRating(Guid ratingId)
    {
        var result = await _mySqlService.ToggleRating(ratingId);
        if (result.Item1)
        {
            return Ok("Rating toggled successfully");
        }
        else
        {
            return BadRequest(result.Item2);
        }
    }
}
