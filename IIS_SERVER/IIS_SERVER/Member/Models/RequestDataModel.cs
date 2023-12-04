/**
* @file RequestDataModel.cs
* author { Martin Kubicka (xkubic45)  Matìj Macek (xmacek27)}
* @date 17.12.2023
* @brief Definition of RequestDataModel
*/

using System.ComponentModel.DataAnnotations;
using IIS_SERVER.Enums;
using IIS_SERVER.Utils;

namespace IIS_SERVER.Member.Models;

public class RequestDataModel
{
    [Required(ErrorMessage = "Handle is required")]
    [StringValidation(ErrorMessage = "The Handle field cannot be empty or contain only whitespace.")]
    public string Handle { get; set; }
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
}
