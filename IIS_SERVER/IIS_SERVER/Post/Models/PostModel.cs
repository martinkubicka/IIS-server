using System.ComponentModel.DataAnnotations;
using IIS_SERVER.Enums;
using IIS_SERVER.Utils;

namespace IIS_SERVER.Post.Models;

/* Id VARCHAR(255) NOT NULL PRIMARY KEY,
    ThreadId VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL,
    Text VARCHAR(255) NOT NULL,
    Date DATETIME NOT NULL,
    FOREIGN KEY (Email) REFERENCES Users(Email),
    FOREIGN KEY (ThreadId) REFERENCES Thread(Id)
 */
public class PostModel
{
    [Required(ErrorMessage = "Id is required")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "ThreadId is required")]
    [StringValidation(
        ErrorMessage = "The ThreadId field cannot be empty or contain only whitespace."
    )]
    public string ThreadId { get; set; }

    [Required(ErrorMessage = "UserEmail is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string UserEmail { get; set; }

    [Required(ErrorMessage = "Text is required")]
    [StringValidation(ErrorMessage = "The Text field cannot be empty or contain only whitespace.")]
    public string Text { get; set; }

    [Required(ErrorMessage = "Date is required")]
    public DateTime Date { get; set; }
}
