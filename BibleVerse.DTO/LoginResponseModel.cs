using System;
using System.ComponentModel.DataAnnotations;

namespace BibleVerse.DTO
{
    public class LoginResponseModel
    {
            [Required]
            public string ResponseStatus { get; set; }

            [Required]
            public Users ResponseUser { get; set; }
    }
}
