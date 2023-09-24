using IIS_SERVER.User.Models;
using MySql.Data.MySqlClient;
using IIS_SERVER.Enums;

namespace IIS_SERVER.Services;

public partial class MySQLService : IMySQLService
{
    public async Task<bool> AddUser(UserDetailModel user)
    {
        try
        {
            string insertQuery =
                "INSERT INTO Users (Email, Password, Handle, Name, Role, Icon) "
                + "VALUES (@Email, @Password, @Handle, @Name, @Role, @Icon)";

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

            return true;
        }
        catch
        {
            return false;
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
                            Icon = reader.IsDBNull(reader.GetOrdinal("Icon"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("Icon"))
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
                            Icon = reader.IsDBNull(reader.GetOrdinal("Icon"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("Icon")),
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

    public async Task<bool> UpdateUser(
        UserDetailModel updatedUser,
        UserPrivacySettingsModel userPrivacy
    )
    {
        try
        {
            string updateQuery =
                @"UPDATE Users SET Name = @Name,Role = @Role,VisibilityRegistered = @VisibilityRegistered,VisibilityGuest = @VisibilityGuest,VisibilityGroup = @VisibilityGroup,Icon = @Icon, Password = @Password WHERE Email = @Email";

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

    public async Task<bool> DeleteUser(string email)
    {
        try
        {
            using (MySqlCommand cmd = new MySqlCommand("CALL DeleteUser(@userEmail)", Connection))
            {
                cmd.Parameters.AddWithValue("@userEmail", email);

                await cmd.ExecuteNonQueryAsync();
            }

            return true;
        }
        catch
        {
            return false;
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
                            VisibilityGroup = reader.GetBoolean(
                                reader.GetOrdinal("VisibilityGroup")
                            ),
                            VisibilityGuest = reader.GetBoolean(
                                reader.GetOrdinal("VisibilityGuest")
                            ),
                            VisibilityRegistered = reader.GetBoolean(
                                reader.GetOrdinal("VisibilityRegistered")
                            )
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
