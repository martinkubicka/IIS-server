/**
* @file IMySQLService.Login.cs
* author { Martin Kubicka (xkubic45) }
* @date 17.12.2023
* @brief Declaration of service for login
*/


using IIS_SERVER.Login.Models;
using IIS_SERVER.User.Models;

namespace IIS_SERVER.Services;

public partial interface IMySQLService
{
    Task<Tuple<bool, string?>> Login(string username, string password);
    Task<Tuple<bool, string?>> ForgotPassword(string email);
    Task<Tuple<bool, string?>> NewPassword(NewPasswordModel data);
}
