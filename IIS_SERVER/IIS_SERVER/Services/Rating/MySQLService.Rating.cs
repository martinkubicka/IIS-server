using IIS_SERVER.Rating.Models;
using MySql.Data.MySqlClient;

namespace IIS_SERVER.Services;

public partial class MySQLService : IMySQLService
{
    public async Task<RatingModel?> GetRating(Guid ratingId)
    {
        var query = "SELECT * FROM Rating WHERE Id = @ratingId";
        using (var command = new MySqlCommand(query, Connection))
        {
            command.Parameters.AddWithValue("@ratingId", ratingId.ToString());

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new RatingModel
                    {
                        Id = Guid.Parse(reader["Id"].ToString()),
                        Rating = reader.GetBoolean(reader.GetOrdinal("Rating")),
                        PostId = Guid.Parse(reader["PostId"].ToString()),
                        Email = reader["Email"].ToString()
                    };
                }
            }
        }

        return null;
    }

    public async Task<List<RatingModel?>> GetRatings(Guid postId)
    {
        var ratings = new List<RatingModel?>();

        var query = "SELECT * FROM Rating WHERE PostId = @postId";
        using (var command = new MySqlCommand(query, Connection))
        {
            command.Parameters.AddWithValue("@postId", postId.ToString());

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var rating = new RatingModel
                    {
                        Id = Guid.Parse(reader["Id"].ToString()),
                        Rating = reader.GetBoolean(reader.GetOrdinal("Rating")),
                        PostId = Guid.Parse(reader["PostId"].ToString()),
                        Email = reader["Email"].ToString()
                    };
                    ratings.Add(rating);
                }
            }
        }

        return ratings;
    }

    public async Task<Tuple<bool, string?>> AddRating(RatingModel rating)
    {
        try
        {
            var insertQuery =
                "INSERT INTO Rating (Id, Rating, PostId, Email) "
                + "VALUES (@Id, @Rating, @PostId, @Email)";

            using (var command = new MySqlCommand(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("@Id", rating.Id.ToString());
                command.Parameters.AddWithValue("@Rating", rating.Rating);
                command.Parameters.AddWithValue("@PostId", rating.PostId.ToString());
                command.Parameters.AddWithValue("@Email", rating.Email);

                await command.ExecuteNonQueryAsync();
            }

            return Tuple.Create(true, (string?)null);
        }
        catch (Exception ex)
        {
            return Tuple.Create(false, ex.Message);
        }
    }

    public async Task<Tuple<bool, string?>> RemoveRating(Guid ratingId)
    {
        try
        {
            var deleteQuery = "DELETE FROM Rating WHERE Id = @ratingId";

            using (var command = new MySqlCommand(deleteQuery, Connection))
            {
                command.Parameters.AddWithValue("@ratingId", ratingId.ToString());

                await command.ExecuteNonQueryAsync();
            }

            return Tuple.Create(true, (string?)null);
        }
        catch (Exception ex)
        {
            return Tuple.Create(false, ex.Message);
        }
    }

    public async Task<Tuple<bool, string?>> ToggleRating(Guid ratingId)
    {
        try
        {
            var updateQuery = "UPDATE Rating SET Rating = NOT Rating WHERE Id = @ratingId";

            using (var command = new MySqlCommand(updateQuery, Connection))
            {
                command.Parameters.AddWithValue("@ratingId", ratingId.ToString());

                await command.ExecuteNonQueryAsync();
            }

            return Tuple.Create(true, (string?)null);
        }
        catch (Exception ex)
        {
            return Tuple.Create(false, ex.Message);
        }
    }
}
