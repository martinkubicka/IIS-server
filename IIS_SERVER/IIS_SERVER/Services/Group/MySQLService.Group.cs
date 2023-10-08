using IIS_SERVER.Group.Models;
using MySql.Data.MySqlClient;

namespace IIS_SERVER.Services;

public partial class MySQLService : IMySQLService
{
    public async Task<bool> AddGroup(GroupListModel group)
    {
        try
        {
            string insertQuery =
                "INSERT INTO `Groups` (Handle, Name, Description, Icon) "
                + "VALUES (@Handle, @Name, @Description, @Icon)";

            using (MySqlCommand cmd = new MySqlCommand(insertQuery, Connection))
            {
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

    public async Task<GroupListModel?> GetGroup(string handle)
    {
        try
        {
            string selectQuery = "SELECT * FROM `Groups` WHERE Handle = @Handle";

            using (MySqlCommand cmd = new MySqlCommand(selectQuery, Connection))
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
        catch
        {
            return null;
        }
    }
    
    public async Task<GroupPrivacySettingsModel?> GetGroupPolicy(string handle)
    {
        try
        {
            string selectQuery = "SELECT * FROM `Groups` WHERE Handle = @Handle";

            using (MySqlCommand cmd = new MySqlCommand(selectQuery, Connection))
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
                            VisibilityGuest = reader.GetBoolean(reader.GetOrdinal("VisibilityGuest")),
                            VisibilityMember = reader.GetBoolean(reader.GetOrdinal("VisibilityMember")),
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

    public async Task<List<GroupListModel?>> GetGroups()
    {
        try
        {
            string selectQuery = "SELECT * FROM `Groups`";

            using (MySqlCommand cmd = new MySqlCommand(selectQuery, Connection))
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

    public async Task<List<GroupListModel>> GetGroups(string userEmail, bool joined)
    {
        try
        {
            string selectQuery = joined
                ? "SELECT G.* FROM `Groups` G INNER JOIN Member M ON G.Handle = M.Handle WHERE M.Email = @Email"
                : "SELECT * FROM `Groups` WHERE Handle NOT IN (SELECT Handle FROM Member WHERE Email = @Email)";

            using (MySqlCommand cmd = new MySqlCommand(selectQuery, Connection))
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

    public async Task<bool> DeleteGroup(string handle)
    {
        try
        {
            string deleteQuery = "DELETE FROM `Groups` WHERE Handle = @Handle";

            using (MySqlCommand cmd = new MySqlCommand(deleteQuery, Connection))
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

    public async Task<bool> UpdateGroup(GroupListModel listModel)
    {
        try
        {
            string updateQuery =
                "UPDATE `Groups` "
                + "SET Name = @Name, Description = @Description, Icon = @Icon "
                + "WHERE Handle = @Handle";

            using (MySqlCommand cmd = new MySqlCommand(updateQuery, Connection))
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

    public async Task<bool> UpdateGroupPolicy(GroupPrivacySettingsModel privacySettingsModel, string handle)
    {
        try
        {
            string updateQuery =
                "UPDATE `Groups` "
                + "SET VisibilityMember = @VisibilityMember, VisibilityGuest = @VisibilityGuest "
                + "WHERE Handle = @Handle";

            using (MySqlCommand cmd = new MySqlCommand(updateQuery, Connection))
            {
                cmd.Parameters.AddWithValue(
                    "@VisibilityMember",
                    privacySettingsModel.VisibilityMember
                );
                cmd.Parameters.AddWithValue(
                    "@VisibilityGuest",
                    privacySettingsModel.VisibilityGuest
                );
                cmd.Parameters.AddWithValue(
                    "@Handle",
                    handle
                );

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
