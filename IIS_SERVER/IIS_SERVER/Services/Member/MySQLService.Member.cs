/**
* @file MySQLService.Member.cs
* author { Martin Kubicka (xkubic45)  Mat�j Macek (xmacek27)}
* @date 17.12.2023
* @brief Definition of service for member
*/


using IIS_SERVER.Member.Models;
using IIS_SERVER.Enums;
using IIS_SERVER.User.Models;
using MySql.Data.MySqlClient;

namespace IIS_SERVER.Services;

public partial class MySQLService : IMySQLService
{
    public async Task<Tuple<bool, string?>> AddMember(MemberModel member)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                string insertQuery =
                    "INSERT INTO Member (Id, Handle, Email, GroupRole, Icon, Name) "
                    + "VALUES (@Id, @Handle, @Email, @GroupRole, @Icon, @Name)";

                using (MySqlCommand cmd = new MySqlCommand(insertQuery, NewConnection))
                {
                    cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@Handle", member.Handle);
                    cmd.Parameters.AddWithValue("@Email", member.Email);
                    cmd.Parameters.AddWithValue("@GroupRole", (int)member.Role);
                    cmd.Parameters.AddWithValue("@Icon", member.Icon);
                    cmd.Parameters.AddWithValue("@Name", member.Name);

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
    
    public async Task<List<string>?> GetJoinRequests(string handle)
    {
        List<string> result = new List<string>();
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                var countQuery = "SELECT * FROM JoinRequest WHERE Handle = @Handle";

                using (var countCommand = new MySqlCommand(countQuery, NewConnection))
                {
                    countCommand.Parameters.AddWithValue("@Handle", handle);
                    
                    using (var reader = await countCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(reader.GetString(reader.GetOrdinal("Email")));
                        }
                    }
                }
                
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
    
    public async Task<List<string>?> GetModeratorRequests(string handle)
    {
        List<string> result = new List<string>();
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                var countQuery = "SELECT * FROM ModeratorRequest WHERE Handle = @Handle";

                using (var countCommand = new MySqlCommand(countQuery, NewConnection))
                {
                    countCommand.Parameters.AddWithValue("@Handle", handle);
                    
                    using (var reader = await countCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(reader.GetString(reader.GetOrdinal("Email")));
                        }
                    }
                }
                
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
    
    public async Task<Tuple<bool, string?>> CreateJoinRequest(string handle, string email)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                string insertQuery =
                    "INSERT INTO JoinRequest (Id, Handle, Email) "
                    + "VALUES (@Id, @Handle, @Email)";

                using (MySqlCommand cmd = new MySqlCommand(insertQuery, NewConnection))
                {
                    cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@Handle", handle);
                    cmd.Parameters.AddWithValue("@Email", email);

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
    
    public async Task<bool> JoinRequested(string handle, string email)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                int res = 0;
                string insertQuery =
                    "SELECT COUNT(*) FROM JoinRequest WHERE Handle = @Handle AND Email = @Email";

                using (MySqlCommand cmd = new MySqlCommand(insertQuery, NewConnection))
                {
                    cmd.Parameters.AddWithValue("@Handle", handle);
                    cmd.Parameters.AddWithValue("@Email", email);

                    await cmd.ExecuteNonQueryAsync();
                    res = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }
                
                return res > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
    
    public async  Task<bool> ModeratorRequested(string handle, string email)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                int res = 0;
                string insertQuery =
                    "SELECT COUNT(*) FROM ModeratorRequest WHERE Handle = @Handle AND Email = @Email";

                using (MySqlCommand cmd = new MySqlCommand(insertQuery, NewConnection))
                {
                    cmd.Parameters.AddWithValue("@Handle", handle);
                    cmd.Parameters.AddWithValue("@Email", email);

                    await cmd.ExecuteNonQueryAsync();
                    res = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }
                return res > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
    
    public async Task<Tuple<bool, string?>> CreateModeratorRequest(string handle, string email)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                string insertQuery =
                    "INSERT INTO ModeratorRequest (Id, Handle, Email) "
                    + "VALUES (@Id, @Handle, @Email)";

                using (MySqlCommand cmd = new MySqlCommand(insertQuery, NewConnection))
                {
                    cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@Handle", handle);
                    cmd.Parameters.AddWithValue("@Email", email);

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
    
    public async Task<bool> DeleteModeratorRequest(string email, string handle)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                using (
                    MySqlCommand command = new MySqlCommand(
                        "DELETE FROM ModeratorRequest WHERE Handle = @Handle AND Email = @Email",
                        NewConnection
                    )
                )
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Handle", handle);
                    await command.ExecuteNonQueryAsync();
                }
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
    
    public async Task<bool> DeleteJoinRequest(string email, string handle)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                using (
                    MySqlCommand command = new MySqlCommand(
                        "DELETE FROM JoinRequest WHERE Handle = @Handle AND Email = @Email",
                        NewConnection
                    )
                )
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Handle", handle);
                    await command.ExecuteNonQueryAsync();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public async Task<Tuple<bool, string?>> DeleteMember(string email, string handle)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                using (
                    MySqlCommand command = new MySqlCommand(
                        "CALL DeleteMember(@Email, @Handle)",
                        NewConnection
                    )
                )
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Handle", handle);

                    int rowsAffected = 0;

                    while (true)
                    {
                        try
                        {
                            rowsAffected = await command.ExecuteNonQueryAsync();
                            break;
                        }
                        catch (MySqlException ex)
                        {
                            if (
                                !ex.Message.Contains(
                                    "There is already an open DataReader associated with this Connection which must be closed first"
                                )
                            )
                            {
                                throw;
                            }
                        }
                    }

                    return Tuple.Create(rowsAffected > 0, "Member");
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create(false, ex.Message);
            }
        }
    }

    public async Task<Tuple<bool, string?>> UpdateMemberRole(
        string email,
        GroupRole role,
        string handle
    )
    {
        string query =
            "UPDATE Member SET GroupRole = @GroupRole WHERE Email = @Email AND Handle = @Handle";
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                using (MySqlCommand command = new MySqlCommand(query, NewConnection))
                {
                    command.Parameters.AddWithValue("@GroupRole", (int)role);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Handle", handle);
                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    return Tuple.Create(rowsAffected > 0, "Member");
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create(false, ex.Message);
            }
        }
    }

    public async Task<Tuple<List<MemberModel>?, string?>> GetMembers(
        string handle,
        GroupRole? role,
        int currentPage,
        int itemsPerPage
    )
    {
        string query =
            "SELECT U.*, M.GroupRole FROM Member AS M INNER JOIN Users AS U ON M.Email = U.Email WHERE  ";

        if (role.HasValue)
        {
            query +=
                "M.Handle = @Handle AND M.GroupRole = @GroupRole LIMIT @ItemsPerPage OFFSET @Offset";
        }
        else
        {
            query += "M.Handle = @Handle LIMIT @ItemsPerPage OFFSET @Offset";
        }
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                using (MySqlCommand command = new MySqlCommand(query, NewConnection))
                {
                    command.Parameters.AddWithValue("@Handle", handle);
                    command.Parameters.AddWithValue("@ItemsPerPage", itemsPerPage);
                    command.Parameters.AddWithValue("@Offset", (currentPage - 1) * itemsPerPage);
                    if (role.HasValue)
                    {
                        command.Parameters.AddWithValue("@GroupRole", (int)role);
                    }

                    List<MemberModel> users = new List<MemberModel>();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var user = new MemberModel
                            {
                                Handle = reader.GetString(reader.GetOrdinal("Handle")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Icon = reader.IsDBNull(reader.GetOrdinal("Icon"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("Icon")),
                                Role = (GroupRole)reader.GetInt32(reader.GetOrdinal("GroupRole")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                            };
                            users.Add(user);
                        }

                        if (users.Count == 0)
                        {
                            return Tuple.Create<List<MemberModel>?, string?>(null, "Groups");
                        }

                        return Tuple.Create(users, "");
                    }
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create<List<MemberModel>?, string?>(null, ex.Message);
            }
        }
    }

    public async Task<int?> GetMembersCount(string Handle)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                var totalMembers = 0;

                var countQuery = "SELECT COUNT(*) FROM Member WHERE Handle = @Handle";

                using (var countCommand = new MySqlCommand(countQuery, NewConnection))
                {
                    countCommand.Parameters.AddWithValue("@Handle", Handle);
                    totalMembers = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
                }

                return totalMembers;
            }
            catch
            {
                return null;
            }
        }
    }

    public async Task<bool?> UserInGroup(string email, string handle)
    {
        bool result = false;
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                var countQuery =
                    "SELECT COUNT(*) FROM Member WHERE Handle = @Handle AND Email = @Email";

                using (var countCommand = new MySqlCommand(countQuery, NewConnection))
                {
                    countCommand.Parameters.AddWithValue("@Handle", handle);
                    countCommand.Parameters.AddWithValue("@Email", email);
                    result = Convert.ToInt32(await countCommand.ExecuteScalarAsync()) != 0;
                }

                return result;
            }
            catch
            {
                return null;
            }
        }
    }

    public async Task<GroupRole?> GetMemberRole(string email, string handle)
    {
        GroupRole? result = null;
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                var countQuery = "SELECT * FROM Member WHERE Handle = @Handle AND Email = @Email";

                using (var countCommand = new MySqlCommand(countQuery, NewConnection))
                {
                    countCommand.Parameters.AddWithValue("@Handle", handle);
                    countCommand.Parameters.AddWithValue("@Email", email);

                    using (var reader = await countCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result = (GroupRole)reader.GetInt32(reader.GetOrdinal("GroupRole"));
                        }
                    }
                }

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public async Task<bool> IsMember(string email, string groupHandle)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                var countQuery =
                    "SELECT COUNT(*) FROM Member WHERE Handle = @Handle AND Email = @Email";

                using (var countCommand = new MySqlCommand(countQuery, NewConnection))
                {
                    countCommand.Parameters.AddWithValue("@Handle", groupHandle);
                    countCommand.Parameters.AddWithValue("@Email", email);

                    return Convert.ToInt32(await countCommand.ExecuteScalarAsync()) != 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
