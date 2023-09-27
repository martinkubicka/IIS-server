using IIS_SERVER.Member.Models;
using IIS_SERVER.Enums;
using IIS_SERVER.User.Models;
using MySql.Data.MySqlClient;

namespace IIS_SERVER.Services;

public partial class MySQLService : IMySQLService
{
    public async Task<Tuple<bool, string?>> AddMember(MemberModel member)
    {
        try
        {
            string insertQuery = "INSERT INTO Member (Id, Handle, Email, GroupRole) " +
                                 "VALUES (@Id, @Handle, @Email, @GroupRole)";

            using (MySqlCommand cmd = new MySqlCommand(insertQuery, Connection))
            {
                cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                cmd.Parameters.AddWithValue("@Handle", member.Handle);
                cmd.Parameters.AddWithValue("@Email", member.Email);
                cmd.Parameters.AddWithValue("@GroupRole", member.Role);
                
                await cmd.ExecuteNonQueryAsync();
            }
            
            return Tuple.Create(true, "");
        }
        catch (Exception ex)
        {
            return Tuple.Create(false, ex.Message);
        }
    }

    public async Task<Tuple<bool, string?>> DeleteMember(string email, string handle)
    {
        try
        {
            using (MySqlCommand command = new MySqlCommand("CALL DeleteMember(@Email, @Handle)", Connection))
            {
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

    public async Task<Tuple<bool, string?>> UpdateMemberRole(string email, GroupRole role, string handle)
    {
        string query = "UPDATE Member SET GroupRole = @GroupRole WHERE Email = @Email AND Handle = @Handle";

        try
        {
            using (MySqlCommand command = new MySqlCommand(query, Connection))
            {
                command.Parameters.AddWithValue("@GroupRole", role);
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
    
    public async Task<Tuple<List<UserListModel>?, string?>> GetMembers(string handle, GroupRole? role)
    {
        string query = "SELECT U.* FROM Member AS M INNER JOIN Users AS U ON M.Email = U.Email WHERE  ";
        
        if (role.HasValue)
        {
            query += "M.Handle = @Handle AND M.GroupRole = @GroupRole";
        }
        else
        {
            query += "M.Handle = @Handle";
        }

        try
        {
            using (MySqlCommand command = new MySqlCommand(query, Connection))
            {
                command.Parameters.AddWithValue("@Handle", handle);
                if (role.HasValue)
                {
                    command.Parameters.AddWithValue("@GroupRole", (int)role);
                }

                List<UserListModel> users = new List<UserListModel>();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var user = new UserListModel
                        {
                            Handle = reader.GetString(reader.GetOrdinal("Handle")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Icon = reader.IsDBNull(reader.GetOrdinal("Icon")) ? null : reader.GetString(reader.GetOrdinal("Icon")),
                            Role = (Role)reader.GetInt32(reader.GetOrdinal("Role"))
                        };
                        
                        users.Add(user);
                    }

                    if (users.Count == 0)
                    {
                        return Tuple.Create<List<UserListModel>?, string?>(null, "Groups");
                    }
                    
                    return Tuple.Create(users, "");
                }
                
            }
        }
        catch (Exception ex)
        {
            return Tuple.Create<List<UserListModel>?, string?>(null, ex.Message);
        }
    }
}
