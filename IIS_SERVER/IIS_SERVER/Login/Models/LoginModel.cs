namespace IIS_SERVER.Login.Models;
using System.ComponentModel.DataAnnotations;
using IIS_SERVER.Utils;

public class LoginModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [StringValidation(ErrorMessage = "The Password field cannot be empty or contain only whitespace.")]
    public string Password { get; set; }
}