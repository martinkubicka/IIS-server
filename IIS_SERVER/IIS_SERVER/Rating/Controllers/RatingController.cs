using IIS_SERVER.Services;
using IIS_SERVER.Rating.Models;
using Microsoft.AspNetCore.Mvc;

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