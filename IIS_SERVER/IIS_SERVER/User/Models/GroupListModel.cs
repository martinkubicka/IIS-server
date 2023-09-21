using System.ComponentModel.DataAnnotations;
using IIS_SERVER.Utils;
using IIS_SERVER.Enums;

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
    public string Name { get; set; }

    [Required(ErrorMessage = "Admin is required")]
    public string Admin { get; set; }

    public string? Icon { get; set; }
}
