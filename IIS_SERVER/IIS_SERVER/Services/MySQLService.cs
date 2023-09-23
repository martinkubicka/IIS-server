using IIS_SERVER.User.Models;
using MySql.Data.MySqlClient;
using IIS_SERVER.Enums;
using IIS_SERVER.Member.Models;
using IIS_SERVER.Thread.Models;

namespace IIS_SERVER.Services;

public class MySQLService : IMySQLService, IDisposable
{
    private readonly string ConnectionString;
    private readonly MySqlConnection Connection;
    
    public MySQLService(IConfiguration configuration)
    {
        ConnectionString = configuration["DB:ConnectionString"];
        Connection = new MySqlConnection(ConnectionString);
        Connection.Open();
    }
    
    public void Dispose()
    {
        Connection.Close();
        Connection.Dispose();
    }
    
    public async Task<Tuple<bool, string>> AddUser(UserDetailModel user)
    {
        try
        {
            string insertQuery = "INSERT INTO Users (Email, Password, Handle, Name, Role, Icon) " +
                                 "VALUES (@Email, @Password, @Handle, @Name, @Role, @Icon)";

            using (MySqlCommand cmd = new MySqlCommand(insertQuery, Connection))
            {
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@Handle", user.Handle);
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@Role", user.Role);
                cmd.Parameters.AddWithValue("@Icon", user.Icon);
                
                await cmd.ExecuteNonQueryAsync();
            }
            
            return Tuple.Create(true, "");
        }
        catch (Exception ex)
        {
            return Tuple.Create(false, ex.Message);
        }
    }

    public async Task<List<UserListModel>?> GetUsersList()
    {
        var usersList = new List<UserListModel>();
        try
        {
            var query = "SELECT * FROM Users";
            using (var command = new MySqlCommand(query, Connection))
            {
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
                        
                        usersList.Add(user);
                    }

                    return usersList;
                }
            }
        }
        catch
        {
            return null;
        }
    }

    public async Task<UserListModel?> GetUserProfile(string handle)
    {
        try
        {
            var query = "SELECT * FROM Users WHERE Handle = @Handle";
            using (var command = new MySqlCommand(query, Connection))
            {
                command.Parameters.AddWithValue("@Handle", handle);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new UserListModel
                        {
                            Handle = reader.GetString(reader.GetOrdinal("Handle")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Icon = reader.IsDBNull(reader.GetOrdinal("Icon")) ? null : reader.GetString(reader.GetOrdinal("Icon")),
                            Role = (Role)reader.GetInt32(reader.GetOrdinal("Role"))
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

    public async Task<Role?> GetUserRole(string handle)
    {
        try
        {
            var query = "SELECT * FROM Users WHERE Handle = @Handle";
            using (var command = new MySqlCommand(query, Connection))
            {
                command.Parameters.AddWithValue("@Handle", handle);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return (Role)reader.GetInt32(reader.GetOrdinal("Role"));
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

    public async Task<bool> UpdateUser(UserDetailModel updatedUser, UserPrivacySettingsModel userPrivacy)
    {
        try
        {
            string updateQuery = @"UPDATE Users SET Name = @Name,Role = @Role,VisibilityRegistered = @VisibilityRegistered,VisibilityGuest = @VisibilityGuest,VisibilityGroup = @VisibilityGroup,Icon = @Icon, Password = @Password WHERE Email = @Email";

            MySqlCommand cmd = new MySqlCommand(updateQuery, Connection);
            
            cmd.Parameters.AddWithValue("@Name", updatedUser.Name);
            cmd.Parameters.AddWithValue("@Role", updatedUser.Role);
            cmd.Parameters.AddWithValue("@VisibilityRegistered", userPrivacy.VisibilityRegistered);
            cmd.Parameters.AddWithValue("@VisibilityGuest", userPrivacy.VisibilityGuest);
            cmd.Parameters.AddWithValue("@VisibilityGroup", userPrivacy.VisibilityGroup);
            cmd.Parameters.AddWithValue("@Icon", updatedUser.Icon);
            cmd.Parameters.AddWithValue("@Email", updatedUser.Email);
            cmd.Parameters.AddWithValue("@Password", updatedUser.Password);
            
            await cmd.ExecuteNonQueryAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<Tuple<bool, string?>> DeleteUser(string email)
    {
        try
        {
            using (MySqlCommand cmd = new MySqlCommand("CALL DeleteUser(@userEmail)", Connection))
            {
                cmd.Parameters.AddWithValue("@userEmail", email);

                await cmd.ExecuteNonQueryAsync();
            }

            return Tuple.Create(true, "");
        }
        catch (Exception ex)
        { 
            return Tuple.Create(false, ex.Message);
        }

    }

    public async Task<UserPrivacySettingsModel?> GetUserPrivacySettings(string handle)
    {
        try
        {
            var query = "SELECT * FROM Users WHERE Handle = @Handle";
            using (var command = new MySqlCommand(query, Connection))
            {
                command.Parameters.AddWithValue("@Handle", handle);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new UserPrivacySettingsModel
                        {
                            VisibilityGroup = reader.GetBoolean(reader.GetOrdinal("VisibilityGroup")),
                            VisibilityGuest = reader.GetBoolean(reader.GetOrdinal("VisibilityGuest")),
                            VisibilityRegistered = reader.GetBoolean(reader.GetOrdinal("VisibilityRegistered"))
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
                await command.ExecuteNonQueryAsync();
                
                return Tuple.Create(true, "");
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

                return Tuple.Create(rowsAffected > 0, "");
            }
        }
        catch (Exception ex)
        {
            return Tuple.Create(false, ex.Message);
        }
    }

    public async Task<bool> CreateThread(ThreadModel thread)
    {
        try
        {
            string insertQuery = "INSERT INTO Thread (Id, Handle, Email, Name, Date) " +
                                 "VALUES (@Id, @Handle, @Email, @Name, @Date)";

            using (MySqlCommand cmd = new MySqlCommand(insertQuery, Connection))
            {
                cmd.Parameters.AddWithValue("@Id", thread.Id);
                cmd.Parameters.AddWithValue("@Handle", thread.Handle);
                cmd.Parameters.AddWithValue("@Email", thread.Email);
                cmd.Parameters.AddWithValue("@Name", thread.Name);
                cmd.Parameters.AddWithValue("@Date", thread.Date);

                await cmd.ExecuteNonQueryAsync();
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<ThreadModel>> GetAllThreads()
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

    public async Task<bool> DeleteThread(string threadId)
    {
        try
        {
            using (MySqlCommand cmd = new MySqlCommand("DELETE FROM Thread WHERE Id = @ThreadId", Connection))
            {
                cmd.Parameters.AddWithValue("@ThreadId", threadId);

                await cmd.ExecuteNonQueryAsync();
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}
