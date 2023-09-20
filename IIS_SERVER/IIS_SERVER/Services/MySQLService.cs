using IIS_SERVER.User.Models;
using MySql.Data.MySqlClient;

namespace IIS_SERVER.Services;

public class MySQLService : IMySQLService
{
    private readonly string ConnectionString = "Server=sql11.freesqldatabase.com;Database=sql11647515;Uid=sql11647515;Pwd=I812NztQS8;";
    private readonly MySqlConnection Connection;
    
    public MySQLService()
    {
        Connection = new MySqlConnection(ConnectionString);
        Connection.Open();
    }
    public async Task<bool> AddUser(UserDetailModel user)
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
            
            return true;
        }
        catch
        {
            return false;
        }
    }
}
