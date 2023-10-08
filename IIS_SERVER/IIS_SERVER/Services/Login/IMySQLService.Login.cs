using IIS_SERVER.Login.Models;
using IIS_SERVER.User.Models;

namespace IIS_SERVER.Services;

public partial interface IMySQLService
{
    Task<Tuple<bool, string?>> Login(string username, string password);
    Task<Tuple<bool, string?>> ForgotPassword(string email);
    Task<Tuple<bool, string?>> NewPassword(NewPasswordModel data);
}
