using IIS_SERVER.Member.Models;
using IIS_SERVER.Enums;
using MySql.Data.MySqlClient;

namespace IIS_SERVER.Services;

public partial class MySQLService : IMySQLService
{
    public async Task<Tuple<bool, string?>> AddMember(MemberModel member)
    {
        try
        {
            string insertQuery =
                "INSERT INTO Member (Id, Handle, Email, GroupRole) "
                + "VALUES (@Id, @Handle, @Email, @GroupRole)";

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
            using (
                MySqlCommand command = new MySqlCommand(
                    "CALL DeleteMember(@Email, @Handle)",
                    Connection
                )
            )
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

    public async Task<Tuple<bool, string?>> UpdateMemberRole(
        string email,
        GroupRole role,
        string handle
    )
    {
        string query =
            "UPDATE Member SET GroupRole = @GroupRole WHERE Email = @Email AND Handle = @Handle";

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
}
