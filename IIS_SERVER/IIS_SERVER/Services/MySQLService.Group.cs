using IIS_SERVER.Group.Models;
using MySql.Data.MySqlClient;
using IIS_SERVER.Enums;

namespace IIS_SERVER.Services;

public partial class MySQLService : IMySQLService
{
    public async Task<bool> AddGroup(GroupDetailModel group)
    {
        try
        {
            string insertQuery =
                "INSERT INTO Groups (Email, Password, Handle, Name, Role, Icon) "
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
}
