/**
* @file GroupEmailModel.cs
* @author { Martin Kubicka (xkubic45)  Matìj Macek (xmacek27)}
* @date 17.12.2023
* @brief Defintion of group email model
*/

using System.ComponentModel.DataAnnotations;
namespace IIS_SERVER.Group.Models;

public class GroupEmailModel : GroupListModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
}