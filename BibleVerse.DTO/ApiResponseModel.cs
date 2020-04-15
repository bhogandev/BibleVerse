using System;
using System.Collections.Generic;

namespace BibleVerse.DTO
{
    public class ApiResponseModel
    {
        public string ResponseMessage { get; set; }

        public List<string> ResponseErrors { get; set; }

        public string ResponseBody { get; set; }
    }
}
