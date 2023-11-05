using IIS_SERVER.Group.Models;
using MySql.Data.MySqlClient;

namespace IIS_SERVER.Services;

public partial class MySQLService : IMySQLService
{
    public async Task<bool> AddGroup(GroupListModel group)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                string insertQuery =
                    "INSERT INTO `Groups` (Id, Handle, Name, Description, Icon) "
                    + "VALUES (@Id, @Handle, @Name, @Description, @Icon)";

                using (MySqlCommand cmd = new MySqlCommand(insertQuery, NewConnection))
                {
                    cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@Handle", group.Handle);
                    cmd.Parameters.AddWithValue("@Name", group.Name);
                    cmd.Parameters.AddWithValue("@Description", group.Description);
                    cmd.Parameters.AddWithValue("@Icon", group.Icon);

                    await cmd.ExecuteNonQueryAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }

    public async Task<GroupListModel?> GetGroup(string handle)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                string selectQuery = "SELECT * FROM `Groups` WHERE Handle = @Handle";

                using (MySqlCommand cmd = new MySqlCommand(selectQuery, NewConnection))
                {
                    cmd.Parameters.AddWithValue("@Handle", handle);
                    foreach (MySqlParameter item in cmd.Parameters)
                    {
                        Console.WriteLine(item.Value);
                    }

                    using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new GroupListModel
                            {
                                Handle = reader.GetString("Handle"),
                                Name = reader.GetString("Name"),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                    ? null
                                    : reader.GetString("Description"),
                                Icon = reader.IsDBNull(reader.GetOrdinal("Icon"))
                                    ? null
                                    : reader.GetString("Icon")
                            };
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return null;
            }
        }
    }

    public async Task<List<GroupListModel?>> GetGroupsUserIsIn(string handle)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                string selectQuery = "SELECT G.* FROM `Groups` G INNER JOIN Member M ON G.Handle = M.Handle WHERE M.Email = (SELECT Email FROM Users WHERE Handle = @Handle)";

                using (MySqlCommand cmd = new MySqlCommand(selectQuery, NewConnection))
                {
                    cmd.Parameters.AddWithValue("@Handle", handle);

                    List<GroupListModel> Groups = new List<GroupListModel>();
                    using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Groups.Add(
                                new GroupListModel
                                {
                                    Handle = reader.GetString("Handle"),
                                    Name = reader.GetString("Name"),
                                    Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                        ? null
                                        : reader.GetString("Description"),
                                    Icon = reader.IsDBNull(reader.GetOrdinal("Icon"))
                                        ? null
                                        : reader.GetString("Icon")
                                }
                            );
                        }
                    }

                    return Groups;
                }
            }
            catch
            {
                return new List<GroupListModel?>();
            }
        }
    }

    public async Task<GroupPrivacySettingsModel?> GetGroupPolicy(string handle)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                string selectQuery = "SELECT * FROM `Groups` WHERE Handle = @Handle";

                using (MySqlCommand cmd = new MySqlCommand(selectQuery, NewConnection))
                {
                    cmd.Parameters.AddWithValue("@Handle", handle);
                    foreach (MySqlParameter item in cmd.Parameters)
                    {
                        Console.WriteLine(item.Value);
                    }

                    using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new GroupPrivacySettingsModel
                            {
                                VisibilityGuest = reader.GetBoolean(
                                    reader.GetOrdinal("VisibilityGuest")
                                ),
                                VisibilityMember = reader.GetBoolean(
                                    reader.GetOrdinal("VisibilityMember")
                                ),
                            };
                        }
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }

    public async Task<List<GroupListModel?>> GetGroups()
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                string selectQuery = "SELECT * FROM `Groups`";

                using (MySqlCommand cmd = new MySqlCommand(selectQuery, NewConnection))
                {
                    List<GroupListModel> Groups = new List<GroupListModel>();

                    using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Groups.Add(
                                new GroupListModel
                                {
                                    Handle = reader.GetString("Handle"),
                                    Name = reader.GetString("Name"),
                                    Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                        ? null
                                        : reader.GetString("Description"),
                                    Icon = reader.IsDBNull(reader.GetOrdinal("Icon"))
                                        ? null
                                        : reader.GetString("Icon")
                                }
                            );
                        }
                    }

                    return Groups;
                }
            }
            catch
            {
                return new List<GroupListModel?>();
            }
        }
    }

    public async Task<List<GroupListModel>> GetGroups(string userEmail, bool joined)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                string selectQuery = joined
                    ? "SELECT G.* FROM `Groups` G INNER JOIN Member M ON G.Handle = M.Handle WHERE M.Email = @Email"
                    : "SELECT * FROM `Groups` WHERE Handle NOT IN (SELECT Handle FROM Member WHERE Email = @Email)";

                using (MySqlCommand cmd = new MySqlCommand(selectQuery, NewConnection))
                {
                    cmd.Parameters.AddWithValue("@Email", userEmail);

                    List<GroupListModel> Groups = new List<GroupListModel>();
                    using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Groups.Add(
                                new GroupListModel
                                {
                                    Handle = reader.GetString("Handle"),
                                    Name = reader.GetString("Name"),
                                    Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                        ? null
                                        : reader.GetString("Description"),
                                    Icon = reader.IsDBNull(reader.GetOrdinal("Icon"))
                                        ? null
                                        : reader.GetString("Icon")
                                }
                            );
                        }
                    }

                    return Groups;
                }
            }
            catch
            {
                return new List<GroupListModel>();
            }
        }
    }

    public async Task<bool> DeleteGroup(string handle)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                string deleteQuery = "DELETE FROM `Groups` WHERE Handle = @Handle";

                using (MySqlCommand cmd = new MySqlCommand(deleteQuery, NewConnection))
                {
                    cmd.Parameters.AddWithValue("@Handle", handle);
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    return rowsAffected > 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }

    public async Task<bool> UpdateGroup(GroupListModel listModel)
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                string updateQuery =
                    "UPDATE `Groups` "
                    + "SET Name = @Name, Description = @Description, Icon = @Icon "
                    + "WHERE Handle = @Handle";

                using (MySqlCommand cmd = new MySqlCommand(updateQuery, NewConnection))
                {
                    cmd.Parameters.AddWithValue("@Name", listModel.Name);
                    cmd.Parameters.AddWithValue("@Description", listModel.Description);
                    cmd.Parameters.AddWithValue("@Icon", listModel.Icon);
                    cmd.Parameters.AddWithValue("@Handle", listModel.Handle);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    return rowsAffected > 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }

    public async Task<bool> UpdateGroupPolicy(
        GroupPrivacySettingsModel privacySettingsModel,
        string handle
    )
    {
        using (var NewConnection = new MySqlConnection(ConnectionString))
        {
            NewConnection.Open();
            try
            {
                string updateQuery =
                    "UPDATE `Groups` "
                    + "SET VisibilityMember = @VisibilityMember, VisibilityGuest = @VisibilityGuest "
                    + "WHERE Handle = @Handle";

                using (MySqlCommand cmd = new MySqlCommand(updateQuery, NewConnection))
                {
                    cmd.Parameters.AddWithValue(
                        "@VisibilityMember",
                        privacySettingsModel.VisibilityMember
                    );
                    cmd.Parameters.AddWithValue(
                        "@VisibilityGuest",
                        privacySettingsModel.VisibilityGuest
                    );
                    cmd.Parameters.AddWithValue("@Handle", handle);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    return rowsAffected > 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
