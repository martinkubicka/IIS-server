/**
* @file NewPasswordModel.cs
* @author { Martin Kubicka (xkubic45) }
* @date 17.12.2023
* @brief Definition of new password model
*/

namespace IIS_SERVER.Login.Models;
using System.ComponentModel.DataAnnotations;
using IIS_SERVER.Utils;

public class NewPasswordModel
{
    [Required(ErrorMessage = "Password is required")]
    [StringValidation(ErrorMessage = "The Password field cannot be empty or contain only whitespace.")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "Token is required")]
    public string Token { get; set; }
}