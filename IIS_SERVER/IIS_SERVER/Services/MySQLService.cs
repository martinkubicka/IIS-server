/**
* @file MySQLService.cs
* author { Martin Kubicka (xkubic45), Dominik Petrik (xpetri25), Matej Macek (xmacek27) }
* @date 17.12.2023
* @brief Main file for MySQL service
*/


using MySql.Data.MySqlClient;

namespace IIS_SERVER.Services;

public partial class MySQLService : IDisposable
{
    private readonly string ConnectionString;
    private readonly MySqlConnection Connection;
    private readonly IConfiguration Configuration;

    public MySQLService(IConfiguration configuration)
    {
        ConnectionString = configuration["DB_ConnectionString"];
        Connection = new MySqlConnection(ConnectionString);
        Configuration = configuration;
        
        Connection.Open();
    }

    public void Dispose()
    {
        Connection.Close();
        Connection.Dispose();
    }
}
