/**
* @file IRatingController.cs
* author { Dominik Petrik (xpetri25) }
* @date 17.12.2023
* @brief Declaration of controller for rating endpoints
*/

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
