using IIS_SERVER.Thread.Models;
using MySql.Data.MySqlClient;

namespace IIS_SERVER.Services;

public partial class MySQLService : IMySQLService
{
    public async Task<Tuple<bool, string?>> CreateThread(ThreadModel thread)
    {
        try
        {
            string insertQuery =
                "INSERT INTO Thread (Id, Handle, Email, Name, Date) "
                + "VALUES (@Id, @Handle, @Email, @Name, @Date)";

            using (MySqlCommand cmd = new MySqlCommand(insertQuery, Connection))
            {
                cmd.Parameters.AddWithValue("@Id", thread.Id);
                cmd.Parameters.AddWithValue("@Handle", thread.Handle);
                cmd.Parameters.AddWithValue("@Email", thread.Email);
                cmd.Parameters.AddWithValue("@Name", thread.Name);
                cmd.Parameters.AddWithValue("@Date", thread.Date);

                await cmd.ExecuteNonQueryAsync();
            }

            return Tuple.Create(true, "");
        }
        catch (Exception ex)
        {
            return Tuple.Create(false, ex.Message);
        }
    }

    public async Task<List<ThreadModel>?> GetAllThreads()
    {
        try
        {
            var threads = new List<ThreadModel>();
            var query = "SELECT * FROM Thread"; // Assuming your table name is 'Thread'

            using (var command = new MySqlCommand(query, Connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var thread = new ThreadModel
                        {
                            Id = reader.GetString(reader.GetOrdinal("Id")),
                            Handle = reader.GetString(reader.GetOrdinal("Handle")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date"))
                            // Add other properties as needed
                        };

                        threads.Add(thread);
                    }
                }
            }

            return threads;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<ThreadModel?> GetThread(string threadId)
    {
        try
        {
            var query = "SELECT * FROM Thread WHERE Id = @ThreadId";
            using (var command = new MySqlCommand(query, Connection))
            {
                command.Parameters.AddWithValue("@ThreadId", threadId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new ThreadModel
                        {
                            Id = reader.GetString(reader.GetOrdinal("Id")),
                            Handle = reader.GetString(reader.GetOrdinal("Handle")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date"))
                        };
                    }

                    return null;
                }
            }
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateThread(string threadId, ThreadModel updatedThread)
    {
        try
        {
            string updateQuery = @"UPDATE Thread SET Name = @Name WHERE Id = @ThreadId";

            MySqlCommand cmd = new MySqlCommand(updateQuery, Connection);

            cmd.Parameters.AddWithValue("@Name", updatedThread.Name);
            cmd.Parameters.AddWithValue("@ThreadId", threadId);

            await cmd.ExecuteNonQueryAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<Tuple<bool, string?>> DeleteThread(string threadId)
    {
        try
        {
            using (
                MySqlCommand cmd = new MySqlCommand(
                    "DELETE FROM Thread WHERE Id = @ThreadId",
                    Connection
                )
            )
            {
                cmd.Parameters.AddWithValue("@ThreadId", threadId);

                await cmd.ExecuteNonQueryAsync();
            }
            return Tuple.Create(true, "");
        }
        catch (Exception ex)
        {
            return Tuple.Create(false, ex.Message);
        }
    }
}
