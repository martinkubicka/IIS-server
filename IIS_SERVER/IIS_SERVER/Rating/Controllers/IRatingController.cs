using IIS_SERVER.Rating.Models;
using Microsoft.AspNetCore.Mvc;

namespace IIS_SERVER.Rating.Controllers;

public interface IPostController
{
    Task<IActionResult> GetRating(Guid ratingId);

    Task<IActionResult> GetRatings(Guid postId);

    Task<IActionResult> AddRating(RatingModel rating);

    Task<IActionResult> RemoveRating(Guid ratingId);

    Task<IActionResult> ToggleRating(Guid ratingId);
}
