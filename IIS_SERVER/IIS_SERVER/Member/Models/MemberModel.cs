using System.ComponentModel.DataAnnotations;
using IIS_SERVER.Enums;
using IIS_SERVER.Utils;

namespace IIS_SERVER.Member.Models;

public class MemberModel
{
    [Required(ErrorMessage = "Id is required")]
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Handle is required")]
    [StringValidation(ErrorMessage = "The Handle field cannot be empty or contain only whitespace.")]
    public string Handle { get; set; }
    
    [Required(ErrorMessage = "Role is required")]
    public GroupRole Role { get; set; }
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
    
    public string? Icon { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    [StringValidation(ErrorMessage = "The Name field cannot be empty or contain only whitespace.")]
    public string Name { get; set; }
}
