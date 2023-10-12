using IIS_SERVER.Thread.Models;
using MySql.Data.MySqlClient;

namespace IIS_SERVER.Services;

public partial class MySQLService : IMySQLService
{
    public async Task<Tuple<bool, string?>> CreateThread(ThreadModel thread)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                string insertQuery =
                    "INSERT INTO Thread (Id, Handle, Email, Name, Date, Description) "
                    + "VALUES (@Id, @Handle, @Email, @Name, @Date, @Description)";

                using (MySqlCommand cmd = new MySqlCommand(insertQuery, NewConnection))
                {
                    cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@Handle", thread.Handle);
                    cmd.Parameters.AddWithValue("@Email", thread.Email);
                    cmd.Parameters.AddWithValue("@Name", thread.Name);
                    cmd.Parameters.AddWithValue("@Date", DateTime.Now);
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
    }

    public async Task<List<ThreadModel>?> GetAllThreads()
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                var threads = new List<ThreadModel>();
                var query = "SELECT * FROM Thread";

                using (var command = new MySqlCommand(query, NewConnection))
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
    }

    public async Task<List<ThreadModel>?> GetThreadsFromSpecificGroup(
        string Handle,
        int currentPage,
        int itemsPerPage,
        string? filterName,
        string? filterFromDate,
        string? filterToDate
    )
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                var threads = new List<ThreadModel>();

                var query = "SELECT * FROM Thread WHERE Handle = @Handle";

                var filterClauses = new List<string>();
                if (!string.IsNullOrEmpty(filterName))
                {
                    filterClauses.Add("Name LIKE @FilterName");
                }
                if (!string.IsNullOrEmpty(filterFromDate))
                {
                    filterClauses.Add("Date >= @FilterFromDate");
                }
                if (!string.IsNullOrEmpty(filterToDate))
                {
                    filterClauses.Add("Date <= @FilterToDate");
                }

                if (filterClauses.Count > 0)
                {
                    query += " AND " + string.Join(" AND ", filterClauses);
                }

                query += " ORDER BY Date DESC LIMIT @ItemsPerPage OFFSET @Offset";

                using (var command = new MySqlCommand(query, NewConnection))
                {
                    command.Parameters.AddWithValue("@Handle", Handle);
                    command.Parameters.AddWithValue("@ItemsPerPage", itemsPerPage);
                    command.Parameters.AddWithValue("@Offset", (currentPage - 1) * itemsPerPage);

                    if (!string.IsNullOrEmpty(filterName))
                    {
                        command.Parameters.AddWithValue("@FilterName", "%" + filterName + "%");
                    }
                    if (!string.IsNullOrEmpty(filterFromDate))
                    {
                        command.Parameters.AddWithValue(
                            "@FilterFromDate",
                            DateTime.Parse(filterFromDate)
                        );
                    }
                    if (!string.IsNullOrEmpty(filterToDate))
                    {
                        command.Parameters.AddWithValue(
                            "@FilterToDate",
                            DateTime.Parse(filterToDate)
                        );
                    }

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
    }

    public async Task<int?> GetThreadsCount(
        string Handle,
        string? filterName,
        string? filterFromDate,
        string? filterToDate
    )
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                var totalThreads = 0;

                var countQuery = "SELECT COUNT(*) FROM Thread WHERE Handle = @Handle";

                var filterClauses = new List<string>();
                if (!string.IsNullOrEmpty(filterName))
                {
                    filterClauses.Add("Name LIKE @FilterName");
                }
                if (!string.IsNullOrEmpty(filterFromDate))
                {
                    filterClauses.Add("Date >= @FilterFromDate");
                }
                if (!string.IsNullOrEmpty(filterToDate))
                {
                    filterClauses.Add("Date <= @FilterToDate");
                }

                if (filterClauses.Count > 0)
                {
                    countQuery += " AND " + string.Join(" AND ", filterClauses);
                }

                using (var countCommand = new MySqlCommand(countQuery, NewConnection))
                {
                    countCommand.Parameters.AddWithValue("@Handle", Handle);

                    if (!string.IsNullOrEmpty(filterName))
                    {
                        countCommand.Parameters.AddWithValue("@FilterName", "%" + filterName + "%");
                    }
                    if (!string.IsNullOrEmpty(filterFromDate))
                    {
                        countCommand.Parameters.AddWithValue(
                            "@FilterFromDate",
                            DateTime.Parse(filterFromDate)
                        );
                    }
                    if (!string.IsNullOrEmpty(filterToDate))
                    {
                        countCommand.Parameters.AddWithValue(
                            "@FilterToDate",
                            DateTime.Parse(filterToDate)
                        );
                    }

                    totalThreads = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
                }

                return totalThreads;
            }
            catch
            {
                return null;
            }
        }
    }

    public async Task<ThreadModel?> GetThread(Guid threadId)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                var query = "SELECT * FROM Thread WHERE Id = @ThreadId";
                using (var command = new MySqlCommand(query, NewConnection))
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
    }

    public async Task<bool> UpdateThread(Guid threadId, ThreadModel updatedThread)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                string updateQuery =
                    "UPDATE Thread SET Name = @Name, Description = @Description WHERE Id = @ThreadId";

                MySqlCommand cmd = new MySqlCommand(updateQuery, NewConnection);

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
    }

    public async Task<Tuple<bool, string?>> DeleteThread(Guid threadId)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                using (
                    MySqlCommand cmd = new MySqlCommand(
                        "DELETE FROM Thread WHERE Id = @ThreadId",
                        NewConnection
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

    public async Task<List<ThreadModel>?> GetAllThreadsUserIsIn(string Email)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                var threads = new List<ThreadModel>();

                var query =
                    "SELECT t.* "
                    + "FROM Thread t "
                    + "INNER JOIN Member m ON t.Handle = m.Handle "
                    + "WHERE m.Email = @Email";

                using (var command = new MySqlCommand(query, NewConnection))
                {
                    command.Parameters.AddWithValue("@Email", Email);

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
    }
}
