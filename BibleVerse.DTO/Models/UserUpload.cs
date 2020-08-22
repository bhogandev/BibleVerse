using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace BibleVerse.DTO
{
    public class UserUpload
    {
        public string userID { get; set; }

        public List<string> UploadFiles { get; set; }

        public List<string> FileNames { get; set; }

        public List<string> FileTypes { get; set; }

    }
}
