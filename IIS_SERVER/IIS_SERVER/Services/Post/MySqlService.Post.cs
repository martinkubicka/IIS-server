using IIS_SERVER.Post.Models;
using IIS_SERVER.Enums;
using MySql.Data.MySqlClient;

namespace IIS_SERVER.Services;

public partial class MySQLService : IMySQLService
{

    public async Task<PostModel?> GetPost(Guid postId)
{
    try
    {
        string selectQuery = "SELECT * FROM Post WHERE Id = @Id";
        MySqlCommand cmd = new MySqlCommand(selectQuery, Connection);
        cmd.Parameters.AddWithValue("@Id", postId);
        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new PostModel
            {
                Id = Guid.Parse(reader["Id"].ToString()),
                ThreadId = reader["ThreadId"].ToString(),
                UserEmail = reader["Email"].ToString(),
                Text = reader["Text"].ToString(),
                Date = DateTime.Parse(reader["Date"].ToString())
            };
        }
        return null;
    }
    catch
    {
        return null;
    }
}

public async Task<List<PostModel?>> GetPostsByThread(Guid threadId)
{
    try
    {
        // Retrieve posts by threadId from the Post table
        string selectQuery = "SELECT * FROM Post WHERE ThreadId = @ThreadId";
        MySqlCommand cmd = new MySqlCommand(selectQuery, Connection);
        cmd.Parameters.AddWithValue("@ThreadId", threadId);
        var posts = new List<PostModel?>();

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            posts.Add(new PostModel
            {
                Id = Guid.Parse(reader["Id"].ToString()),
                ThreadId = reader["ThreadId"].ToString(),
                UserEmail = reader["Email"].ToString(),
                Text = reader["Text"].ToString(),
                Date = DateTime.Parse(reader["Date"].ToString())
            });
        }

        return posts;
    }
    catch 
    {
        return new List<PostModel?>();
    }
}

public async Task<List<PostModel?>> GetPostsByUser(string userEmail)
{
    try
    {
        // Retrieve posts by userEmail from the Post table
        string selectQuery = "SELECT * FROM Post WHERE Email = @Email";
        MySqlCommand cmd = new MySqlCommand(selectQuery, Connection);
        cmd.Parameters.AddWithValue("@Email", userEmail);
        var posts = new List<PostModel?>();

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            posts.Add(new PostModel
            {
                Id = Guid.Parse(reader["Id"].ToString()),
                ThreadId = reader["ThreadId"].ToString(),
                UserEmail = reader["Email"].ToString(),
                Text = reader["Text"].ToString(),
                Date = DateTime.Parse(reader["Date"].ToString())
            });
        }

        return posts;
    }
    catch 
    {
        return new List<PostModel?>();
    }
}
    public async Task<Tuple<bool, string?>> AddPost(PostModel post)
    {
        try
        {
            // Insert the post into the Post table
            string insertQuery =
                "INSERT INTO Post (Id, ThreadId, Email, Text, Date) VALUES (@Id, @ThreadId, @Email, @Text, @Date)";
            MySqlCommand cmd = new MySqlCommand(insertQuery, Connection);
            cmd.Parameters.AddWithValue("@Id", post.Id);
            cmd.Parameters.AddWithValue("@ThreadId", post.ThreadId);
            cmd.Parameters.AddWithValue("@Email", post.UserEmail);
            cmd.Parameters.AddWithValue("@Text", post.Text);
            cmd.Parameters.AddWithValue("@Date", post.Date);
            await cmd.ExecuteNonQueryAsync();

            return new Tuple<bool, string?>(true, null);
        }
        catch (Exception ex)
        {
            return new Tuple<bool, string?>(false, ex.Message);
        }
    }

    public async Task<Tuple<bool, string?>> EditPostText(Guid postId, string text)
    {
        try
        {
            // Update the text of the post in the Post table
            string updateQuery = "UPDATE Post SET Text = @Text WHERE Id = @Id";
            MySqlCommand updateCommand = new MySqlCommand(updateQuery, Connection);
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

    public async Task<Tuple<bool, string?>> EditPost(PostModel post)
    {
        try
        {
            // Update the post in the Post table
            string updateQuery =
                "UPDATE Post SET ThreadId = @ThreadId, Email = @Email, Text = @Text, Date = @Date WHERE Id = @Id";
            MySqlCommand updateCommand = new MySqlCommand(updateQuery, Connection);
            updateCommand.Parameters.AddWithValue("@ThreadId", post.ThreadId);
            updateCommand.Parameters.AddWithValue("@Email", post.UserEmail);
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

    public async Task<Tuple<bool, string?>> DeletePost(PostModel post)
    {
        try
        {
            // Delete the post from the Post table
            string deleteQuery = "DELETE FROM Post WHERE Id = @Id";
            MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, Connection);
            deleteCommand.Parameters.AddWithValue("@Id", post.Id);
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
