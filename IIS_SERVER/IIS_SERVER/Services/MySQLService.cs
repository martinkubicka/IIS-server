using IIS_SERVER.User.Models;
using MySql.Data.MySqlClient;
using IIS_SERVER.Enums;

namespace IIS_SERVER.Services;

public partial class MySQLService : IDisposable
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
}
