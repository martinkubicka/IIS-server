using IIS_SERVER.Post.Models;
using IIS_SERVER.Enums;
using System.Data;
using MySql.Data.MySqlClient;

namespace IIS_SERVER.Services;

public partial class MySQLService : IMySQLService
{
    public async Task<PostModel?> GetPost(Guid postId)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                string selectQuery = "SELECT * FROM Post WHERE Id = @Id";
                MySqlCommand cmd = new MySqlCommand(selectQuery, NewConnection);
                cmd.Parameters.AddWithValue("@Id", postId);
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new PostModel
                    {
                        Id = Guid.Parse(reader["Id"].ToString()),
                        ThreadId = Guid.Parse(reader["ThreadId"].ToString()),
                        Handle = reader["Handle"].ToString(),
                        Text = reader["Text"].ToString(),
                        Date = DateTime.Parse(reader["Date"].ToString())
                    };
                }
                reader.Close();

                return null;
            }
            catch
            {
                return null;
            }
        }
    }

    public async Task<List<PostModel?>> GetPostsByThread(Guid threadId, int limit, int offset)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                // Retrieve posts by threadId from the Post table
                string selectQuery =
                    "SELECT * FROM Post WHERE ThreadId = @ThreadId ORDER BY Post.Date DESC LIMIT @offset, @limit";
                MySqlCommand cmd = new MySqlCommand(selectQuery, NewConnection);
                cmd.Parameters.AddWithValue("@ThreadId", threadId);
                cmd.Parameters.AddWithValue("@offset", offset);
                cmd.Parameters.AddWithValue("@limit", limit);
                var posts = new List<PostModel?>();

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    posts.Add(
                        new PostModel
                        {
                            Id = Guid.Parse(reader["Id"].ToString()),
                            ThreadId = Guid.Parse(reader["ThreadId"].ToString()),
                            Handle = reader["Handle"].ToString(),
                            Text = reader["Text"].ToString(),
                            Date = DateTime.Parse(reader["Date"].ToString())
                        }
                    );
                }
                reader.Close();

                return posts;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new List<PostModel?>();
            }
        }
    }

    public async Task<List<PostModel?>> GetPostsByUser(string Handle)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                // Retrieve posts by Handle from the Post table
                string selectQuery = "SELECT * FROM Post WHERE Handle = @Handle";
                MySqlCommand cmd = new MySqlCommand(selectQuery, NewConnection);
                cmd.Parameters.AddWithValue("@Handle", Handle);
                var posts = new List<PostModel?>();

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    posts.Add(
                        new PostModel
                        {
                            Id = Guid.Parse(reader["Id"].ToString()),
                            ThreadId = Guid.Parse(reader["ThreadId"].ToString()),
                            Handle = reader["Handle"].ToString(),
                            Text = reader["Text"].ToString(),
                            Date = DateTime.Parse(reader["Date"].ToString())
                        }
                    );
                }

                return posts;
            }
            catch
            {
                return new List<PostModel?>();
            }
        }
    }

    public async Task<Tuple<bool, string?>> AddPost(PostModel post)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                // Insert the post into the Post table
                string insertQuery =
                    "INSERT INTO Post (Id, ThreadId, Handle, Text, Date) VALUES (@Id, @ThreadId, @Handle, @Text, @Date)";
                MySqlCommand cmd = new MySqlCommand(insertQuery, NewConnection);
                cmd.Parameters.AddWithValue("@Id", post.Id);
                cmd.Parameters.AddWithValue("@ThreadId", post.ThreadId);
                cmd.Parameters.AddWithValue("@Handle", post.Handle);
                cmd.Parameters.AddWithValue("@Text", post.Text);
                cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                await cmd.ExecuteNonQueryAsync();

                return new Tuple<bool, string?>(true, null);
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string?>(false, ex.Message);
            }
        }
    }

    public async Task<Tuple<bool, string?>> EditPostText(Guid postId, string text)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                // Update the text of the post in the Post table
                string updateQuery = "UPDATE Post SET Text = @Text WHERE Id = @Id";
                MySqlCommand updateCommand = new MySqlCommand(updateQuery, NewConnection);
                updateCommand.Parameters.AddWithValue("@Text", text);
                updateCommand.Parameters.AddWithValue("@Id", postId);
                int rowsUpdated = await updateCommand.ExecuteNonQueryAsync();

                if (rowsUpdated > 0)
                {
                    return new Tuple<bool, string?>(true, null);
                }
                else
                {
                    return new Tuple<bool, string?>(false, "Post not found.");
                }
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string?>(false, ex.Message);
            }
        }
    }

    public async Task<Tuple<bool, string?>> EditPost(PostModel post)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                // Update the post in the Post table
                string updateQuery =
                    "UPDATE Post SET ThreadId = @ThreadId, Handle = @Handle, Text = @Text, Date = @Date WHERE Id = @Id";
                MySqlCommand updateCommand = new MySqlCommand(updateQuery, NewConnection);
                updateCommand.Parameters.AddWithValue("@ThreadId", post.ThreadId);
                updateCommand.Parameters.AddWithValue("@Handle", post.Handle);
                updateCommand.Parameters.AddWithValue("@Text", post.Text);
                updateCommand.Parameters.AddWithValue("@Date", post.Date);
                updateCommand.Parameters.AddWithValue("@Id", post.Id);
                int rowsUpdated = await updateCommand.ExecuteNonQueryAsync();

                if (rowsUpdated > 0)
                {
                    return new Tuple<bool, string?>(true, null);
                }
                else
                {
                    return new Tuple<bool, string?>(false, "Post not found.");
                }
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string?>(false, ex.Message);
            }
        }
    }

    public async Task<Tuple<bool, string?>> DeletePost(Guid postId)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                // Delete the post from the Post table
                string deleteQuery = "DELETE FROM Post WHERE Id = @Id";
                MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, NewConnection);
                deleteCommand.Parameters.AddWithValue("@Id", postId);
                int rowsDeleted = await deleteCommand.ExecuteNonQueryAsync();

                if (rowsDeleted > 0)
                {
                    return new Tuple<bool, string?>(true, null);
                }
                else
                {
                    return new Tuple<bool, string?>(false, "Post not found.");
                }
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string?>(false, ex.Message);
            }
        }
    }

    public async Task<int?> CalculateRating(Guid postId)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                string procedureName = "CalculateRating";
                MySqlCommand cmd = new MySqlCommand(procedureName, NewConnection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@postId", postId);

                MySqlParameter ratingCountParam = new MySqlParameter(
                    "@ratingCount",
                    MySqlDbType.Int32
                );
                ratingCountParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(ratingCountParam);

                await cmd.ExecuteNonQueryAsync();
                return Convert.ToInt32(ratingCountParam.Value);
            }
            catch
            {
                return 0;
            }
        }
    }
}
