using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
namespace BibleVerse.DTO
{
    public class EComResponseModel
    {
        public string ResponseStatus { get; set; }

        public List<IdentityError> ResponseErrors { get; set; }
    }
}
