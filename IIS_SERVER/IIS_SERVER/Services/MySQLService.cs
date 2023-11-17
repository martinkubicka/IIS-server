using MySql.Data.MySqlClient;

namespace IIS_SERVER.Services;

public partial class MySQLService : IDisposable
{
    private readonly string ConnectionString;
    private readonly MySqlConnection Connection;
    private readonly IConfiguration Configuration;

    public MySQLService(IConfiguration configuration)
    {
        ConnectionString = "Server=antioznuk-martinkubicka22-d781.aivencloud.com;Port=15939;Database=defaultdb;Uid=avnadmin;Pwd=AVNS_4iCQ_2BI9PsIL6BZ2nu;";
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
