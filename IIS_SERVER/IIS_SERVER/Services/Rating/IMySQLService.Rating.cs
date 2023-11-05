using IIS_SERVER.Rating.Models;

namespace IIS_SERVER.Services;

public partial interface IMySQLService
{
    Task<RatingModel?> GetRating(Guid ratingId);

    Task<RatingModel?> GetRatingByPost(Guid postId, string userEmail);

    Task<List<RatingModel?>> GetRatings(Guid postId);

    Task<Tuple<bool, string?>> AddRating(RatingModel rating);

    Task<Tuple<bool, string?>> RemoveRating(Guid ratingId);

    Task<Tuple<bool, string?>> UpdateRating(Guid ratingId, int ratingChange);

    Task<Tuple<bool, string?>> ToggleRating(Guid ratingId);
}
