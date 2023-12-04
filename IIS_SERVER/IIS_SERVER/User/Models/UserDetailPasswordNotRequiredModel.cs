/**
* @file UserDetailPasswordNotRequiredModel.cs
* author { Matìj Macek (xmacek27)}
* @date 17.12.2023
* @brief Definition of UserDetailPasswordNotRequiredModel model
*/

using System.ComponentModel.DataAnnotations;
using IIS_SERVER.Utils;

namespace IIS_SERVER.User.Models;

public class UserDetailPasswordNotRequiredModel : UserListModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
    
    public string? Password { get; set; }
}