using System.ComponentModel.DataAnnotations;
namespace IIS_SERVER.Group.Models;

public class GroupEmailModel : GroupListModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
}