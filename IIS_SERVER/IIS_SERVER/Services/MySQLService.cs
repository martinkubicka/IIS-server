using System.Configuration;
using MySql.Data.MySqlClient;

namespace IIS_SERVER.Services;

public partial class MySQLService : IDisposable
{
    private readonly string ConnectionString;
    private readonly MySqlConnection Connection;
    private readonly IConfiguration Configuration;
    
    public MySQLService(IConfiguration configuration)
    {
        ConnectionString = configuration["DB:ConnectionString"];
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
