/**
* @file ThreadModel.cs
* author { Martin Kubicka (xkubic45) Matìj Macek (xmacek27)}
* @date 17.12.2023
* @brief Definition of thread model
*/


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
        [StringLength(50, ErrorMessage = "The Name field cannot exceed 50 characters.")]
        public string Name { get; set; }

        [Required]
        public DateTime Date { get; set; }
        
        [StringLength(100, ErrorMessage = "The Description field cannot exceed 100 characters.")]
        public string Description { get; set; }
    }
}
