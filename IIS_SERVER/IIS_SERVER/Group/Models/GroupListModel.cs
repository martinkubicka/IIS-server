/**
* @file GroupListModel.cs
* @author { Martin Kubicka (xkubic45)  Matìj Macek (xmacek27)}
* @date 17.12.2023
* @brief Defintion of group list model
*/

using System.ComponentModel.DataAnnotations;
using IIS_SERVER.Utils;

namespace IIS_SERVER.Group.Models;

public class GroupListModel
{
    [Required(ErrorMessage = "Handle is required")]
    [StringValidation(
        ErrorMessage = "The Handle field cannot be empty or contain only whitespace."
    )]
    public string Handle { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringValidation(ErrorMessage = "The Name field cannot be empty or contain only whitespace.")]
    [StringLength(20, ErrorMessage = "The Name field cannot exceed 20 characters.")]
    public string Name { get; set; }

    [StringLength(100, ErrorMessage = "The Description field cannot exceed 100 characters.")]
    public string? Description { get; set; }

    public string? Icon { get; set; }
}
