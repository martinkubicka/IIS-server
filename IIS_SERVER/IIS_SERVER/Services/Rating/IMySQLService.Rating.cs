using IIS_SERVER.Rating.Models;

namespace IIS_SERVER.Services;

public partial interface IMySQLService
{
    Task<RatingModel?> GetRating(Guid ratingId);

    Task<List<RatingModel?>> GetRatings(Guid postId);

    Task<Tuple<bool, string?>> AddRating(RatingModel rating);

    Task<Tuple<bool, string?>> RemoveRating(Guid ratingId);

    Task<Tuple<bool, string?>> ToggleRating(Guid ratingId);
}
