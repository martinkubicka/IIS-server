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
                "INSERT INTO Thread (Id, Handle, Email, Name, Date, Description) "
                + "VALUES (@Id, @Handle, @Email, @Name, @Date, @Description)";

            using (MySqlCommand cmd = new MySqlCommand(insertQuery, Connection))
            {
                cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                cmd.Parameters.AddWithValue("@Handle", thread.Handle);
                cmd.Parameters.AddWithValue("@Email", thread.Email);
                cmd.Parameters.AddWithValue("@Name", thread.Name);
                cmd.Parameters.AddWithValue("@Date", thread.Date);
                cmd.Parameters.AddWithValue("@Description", thread.Description);

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
            var query = "SELECT * FROM Thread";

            using (var command = new MySqlCommand(query, Connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var thread = new ThreadModel
                        {
                            Id = reader.GetGuid(reader.GetOrdinal("Id")),
                            Handle = reader.GetString(reader.GetOrdinal("Handle")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
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

    public async Task<List<ThreadModel>?> GetThreadsFromSpecificGroup(string Handle, int currentPage, int itemsPerPage)
    {
        try
        {
            var threads = new List<ThreadModel>();

            var query = "SELECT * FROM Thread WHERE Handle = @Handle LIMIT @ItemsPerPage OFFSET @Offset";

            using (var command = new MySqlCommand(query, Connection))
            {
                command.Parameters.AddWithValue("@Handle", Handle);
                command.Parameters.AddWithValue("@ItemsPerPage", itemsPerPage);
                command.Parameters.AddWithValue("@Offset", (currentPage - 1) * itemsPerPage);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var thread = new ThreadModel
                        {
                            Id = reader.GetGuid(reader.GetOrdinal("Id")),
                            Handle = reader.GetString(reader.GetOrdinal("Handle")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
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
    
    public async Task<int?> GetThreadsCount(string Handle)
    {
        try
        {
            var totalThreads = 0;
            
            var countQuery = "SELECT COUNT(*) FROM Thread WHERE Handle = @Handle";

            using (var countCommand = new MySqlCommand(countQuery, Connection))
            {
                countCommand.Parameters.AddWithValue("@Handle", Handle);
                totalThreads = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
            }

            return totalThreads;
        }
        catch
        {
            return null;
        }
    }


    public async Task<ThreadModel?> GetThread(Guid threadId)
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
                            Id = reader.GetGuid(reader.GetOrdinal("Id")),
                            Handle = reader.GetString(reader.GetOrdinal("Handle")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
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

    public async Task<bool> UpdateThread(Guid threadId, ThreadModel updatedThread)
    {
        try
        {
            string updateQuery = "UPDATE Thread SET Name = @Name, Description = @Description WHERE Id = @ThreadId";

            MySqlCommand cmd = new MySqlCommand(updateQuery, Connection);

            cmd.Parameters.AddWithValue("@Name", updatedThread.Name);
            cmd.Parameters.AddWithValue("@Description", updatedThread.Description);
            cmd.Parameters.AddWithValue("@ThreadId", threadId);

            await cmd.ExecuteNonQueryAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<Tuple<bool, string?>> DeleteThread(Guid threadId)
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
