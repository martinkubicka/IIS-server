using IIS_SERVER.User.Models;

namespace IIS_SERVER.Services;

public interface IMySQLService
{
    Task<bool> AddUser(UserDetailModel user);
}