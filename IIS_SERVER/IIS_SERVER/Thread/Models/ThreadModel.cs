using System;
using System.ComponentModel.DataAnnotations;

namespace IIS_SERVER.Thread.Models
{
    public class ThreadModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Handle { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime Date { get; set; }
        
        public string Description { get; set; }
    }
}
