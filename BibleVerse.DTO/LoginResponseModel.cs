using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BibleVerse.DTO
{
    public class LoginResponseModel
    {
            [Required]
            public string ResponseStatus { get; set; }

            public Users ResponseUser { get; set; }

            public List<Error> ResponseErrors { get; set; }
    }
}
