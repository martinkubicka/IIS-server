using IIS_SERVER.Enums;

namespace IIS_SERVER.User.Models;

public class UserDetailModel : UserListModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public Role Role { get; set; }
}