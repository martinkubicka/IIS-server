/**
* @file RatingModel.cs
* author { Dominik Petrik (xpetri25) }
* @date 17.12.2023
* @brief Definition of rating model
*/

using System.ComponentModel.DataAnnotations;

namespace IIS_SERVER.Rating.Models;

public class RatingModel
{
    [Required(ErrorMessage = "Id is required")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Rating is required")]
    public bool Rating { get; set; }

    [Required(ErrorMessage = "PostId is required")]
    public Guid PostId { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
}
