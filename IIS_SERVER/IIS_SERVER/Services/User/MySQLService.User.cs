using IIS_SERVER.User.Models;
using MySql.Data.MySqlClient;
using IIS_SERVER.Enums;
using BCrypt.Net;

namespace IIS_SERVER.Services;

public partial class MySQLService : IMySQLService
{
    public async Task<Tuple<bool, string>> AddUser(UserDetailModel user)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

                string insertQuery =
                    "INSERT INTO Users (Id, Email, Password, Handle, Name, Role, Icon) "
                    + "VALUES (@Id, @Email, @Password, @Handle, @Name, @Role, @Icon)";

                using (MySqlCommand cmd = new MySqlCommand(insertQuery, NewConnection))
                {
                    cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);
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
    }

    public async Task<List<UserListModel>?> GetUsersList()
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            var usersList = new List<UserListModel>();
            try
            {
                var query = "SELECT * FROM Users";
                using (var command = new MySqlCommand(query, NewConnection))
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
                                    : reader.GetString(reader.GetOrdinal("Icon")),
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
    }

    public async Task<Tuple<UserListModel?, string?>> GetUserProfile(string handle)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                var query = "SELECT * FROM Users WHERE Handle = @Handle";
                using (var command = new MySqlCommand(query, NewConnection))
                {
                    command.Parameters.AddWithValue("@Handle", handle);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var tuple = Tuple.Create(
                                new UserListModel
                                {
                                    Handle = reader.GetString(reader.GetOrdinal("Handle")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Icon = reader.IsDBNull(reader.GetOrdinal("Icon"))
                                        ? null
                                        : reader.GetString(reader.GetOrdinal("Icon")),
                                    Role = (Role)reader.GetInt32(reader.GetOrdinal("Role"))
                                },
                                ""
                            );
                            /* reader.Close(); */

                            return tuple;
                        }
                        /* reader.Close(); */

                        return Tuple.Create((UserListModel)null, "Users");
                    }
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create((UserListModel)null, ex.Message);
            }
        }
    }

    public async Task<Tuple<Role?, string>> GetUserRole(string email)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                var query = "SELECT * FROM Users WHERE Email = @Email";
                using (var command = new MySqlCommand(query, NewConnection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return Tuple.Create(
                                (Role?)reader.GetInt32(reader.GetOrdinal("Role")),
                                ""
                            );
                        }

                        return Tuple.Create((Role?)null, "Users");
                    }
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create((Role?)null, ex.Message);
            }
        }
    }

    public async Task<Tuple<string?, string>> GetUserHandle(string email)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                var query = "SELECT * FROM Users WHERE Email = @Email";
                using (var command = new MySqlCommand(query, NewConnection))
                {
                    command.Parameters.AddWithValue("@Email", email);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return Tuple.Create(
                                reader.GetString(reader.GetOrdinal("Handle")),
                                ""
                            );
                        }

                        return Tuple.Create((string)null, "Users");
                    }
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create((string)null, ex.Message);
            }
        }
    }
    
    public async Task<Tuple<bool, string?>> UpdateUser(
        UserDetailModel updatedUser,
        UserPrivacySettingsModel userPrivacy
    )
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password);
                string updateQuery =
                    @"UPDATE Users SET Name = @Name,Role = @Role,VisibilityRegistered = @VisibilityRegistered,VisibilityGuest = @VisibilityGuest,VisibilityGroup = @VisibilityGroup,Icon = @Icon, Password = @Password WHERE Email = @Email";

                MySqlCommand cmd = new MySqlCommand(updateQuery, NewConnection);

                cmd.Parameters.AddWithValue("@Name", updatedUser.Name);
                cmd.Parameters.AddWithValue("@Role", updatedUser.Role);
                cmd.Parameters.AddWithValue(
                    "@VisibilityRegistered",
                    userPrivacy.VisibilityRegistered
                );
                cmd.Parameters.AddWithValue("@VisibilityGuest", userPrivacy.VisibilityGuest);
                cmd.Parameters.AddWithValue("@VisibilityGroup", userPrivacy.VisibilityGroup);
                cmd.Parameters.AddWithValue("@Icon", updatedUser.Icon);
                cmd.Parameters.AddWithValue("@Email", updatedUser.Email);
                cmd.Parameters.AddWithValue("@Password", hashedPassword);

                await cmd.ExecuteNonQueryAsync();

                return Tuple.Create(true, "");
            }
            catch (Exception ex)
            {
                return Tuple.Create(false, ex.Message);
            }
        }
    }

    public async Task<Tuple<bool, string?>> DeleteUser(string email)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                int rowsAffected = 0;
                using (
                    MySqlCommand cmd = new MySqlCommand(
                        "CALL DeleteUser(@userEmail)",
                        NewConnection
                    )
                )
                {
                    cmd.Parameters.AddWithValue("@userEmail", email);

                    rowsAffected = await cmd.ExecuteNonQueryAsync();
                }

                return Tuple.Create(rowsAffected > 0, "Users");
            }
            catch (Exception ex)
            {
                return Tuple.Create(false, ex.Message);
            }
        }
    }

    public async Task<Tuple<UserPrivacySettingsModel?, string?>> GetUserPrivacySettings(
        string handle
    )
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                var query = "SELECT * FROM Users WHERE Handle = @Handle";
                using (var command = new MySqlCommand(query, NewConnection))
                {
                    command.Parameters.AddWithValue("@Handle", handle);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return Tuple.Create(
                                new UserPrivacySettingsModel
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
                                },
                                ""
                            );
                        }

                        return Tuple.Create((UserPrivacySettingsModel?)null, "Users");
                    }
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create((UserPrivacySettingsModel?)null, ex.Message);
            }
        }
    }
}
