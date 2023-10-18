using System.ComponentModel.DataAnnotations;
using IIS_SERVER.Utils;

namespace IIS_SERVER.User.Models;

public class UserDetailModel : UserListModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [StringValidation(ErrorMessage = "The Password field cannot be empty or contain only whitespace.")]
    [PasswordValidation(ErrorMessage = "The Password field must be at least 8 characters long, contain at least one uppercase letter, one number, and one special character.")]
    public string? Password { get; set; }
}