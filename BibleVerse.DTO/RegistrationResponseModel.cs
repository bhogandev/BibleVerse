using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace BibleVerse.DTO
{
    public class RegistrationResponseModel
    {
        public string ResponseMessage { get; set; }

        public string ConfirmationToken { get; set; }

        public string UserId { get; set; }

        public List<string> ResponseErrors { get; set; }
    }
}
