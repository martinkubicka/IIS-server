using System.ComponentModel.DataAnnotations;
using IIS_SERVER.Enums;

namespace IIS_SERVER.User.Models;

public class UserProfileModel : UserListModel
{
    [Required(ErrorMessage = "Role is required")]
    public Role Role { get; set; }
}