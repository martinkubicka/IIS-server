using System.ComponentModel.DataAnnotations;
using IIS_SERVER.Utils;

namespace IIS_SERVER.Post.Models;

public class PostModel
{
    [Required(ErrorMessage = "Id is required")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "ThreadId is required")]
    [StringValidation(
        ErrorMessage = "The ThreadId field cannot be empty or contain only whitespace."
    )]
    public string ThreadId { get; set; }

    [Required(ErrorMessage = "Handle is required")]
    [StringValidation(ErrorMessage = "The Handle field cannot be empty or contain only whitespace.")]
    public string Handle { get; set; }

    [Required(ErrorMessage = "Text is required")]
    [StringValidation(ErrorMessage = "The Text field cannot be empty or contain only whitespace.")]
    public string Text { get; set; }

    [Required(ErrorMessage = "Date is required")]
    public DateTime Date { get; set; }
}
