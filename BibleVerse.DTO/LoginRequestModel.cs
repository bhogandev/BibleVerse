using System;
using System.ComponentModel.DataAnnotations;

namespace BibleVerse.DTO
{
    public class LoginRequestModel
    {
       [Required]
       public string Email { get; set; }

       [Required]
       public string Password { get; set; }
    }
}
