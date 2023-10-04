using IIS_SERVER.Login.Models;
using IIS_SERVER.User.Models;
using MySql.Data.MySqlClient;

namespace IIS_SERVER.Services;

public partial class MySQLService : IMySQLService
{
    public async Task<Tuple<bool, string?>> Login(string username, string password)
    {
        string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND Password = @Password";
        using MySqlCommand command = new MySqlCommand(query, Connection);
        command.Parameters.AddWithValue("@Email", username);
        command.Parameters.AddWithValue("@Password", password);

        int count = Convert.ToInt32(await command.ExecuteScalarAsync());

        if (count > 0)
        {
            return new Tuple<bool, string?>(true, null);
        }
        else
        {
            return new Tuple<bool, string?>(false, "Invalid email or password.");
        }
        
    }

    public async Task<Tuple<bool, string?>> ForgotPassword(string email)
    {
        try
        {
            var id = Guid.NewGuid();
            string insertQuery = "INSERT INTO Tokens (Email, Token, CreatedAt) VALUES (@Email, @Token, NOW())";
            using MySqlCommand command = new MySqlCommand(insertQuery, Connection);
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@Token", id.ToString());

            await command.ExecuteNonQueryAsync();

            return Tuple.Create(true, id.ToString());
        }
        catch (Exception ex)
        {
            return Tuple.Create(false, ex.Message);
        }
    }

    public async Task<Tuple<bool, string?>> NewPassword(NewPasswordModel data)
    {
        try
        {
            string query = "SELECT Email FROM Tokens WHERE Token = @Token";
            using MySqlCommand command = new MySqlCommand(query, Connection);
            command.Parameters.AddWithValue("@Token", data.Token);

            object result = command.ExecuteScalar();
            var email = result.ToString();
            
            if (string.IsNullOrEmpty(email))
            {
                return Tuple.Create(false, "Invalid or expired token.");
            }
            
            string updateQuery = "UPDATE Users SET Password = @NewPassword WHERE Email = @Email";
            using MySqlCommand updateCommand = new MySqlCommand(updateQuery, Connection);
            updateCommand.Parameters.AddWithValue("@NewPassword", data.Password);
            updateCommand.Parameters.AddWithValue("@Email", email);

            int rowsAffected = await updateCommand.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
            {
                return Tuple.Create(false, "Failed to update password.");
            }
            
            string deleteQuery = "DELETE FROM Tokens WHERE Token = @Token";
            using MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, Connection);
            deleteCommand.Parameters.AddWithValue("@Token", data.Token);

            await deleteCommand.ExecuteNonQueryAsync();

            return Tuple.Create(true, "");
        }
        catch (Exception ex)
        {
            return Tuple.Create(false, ex.Message);
        }  
    }
}
