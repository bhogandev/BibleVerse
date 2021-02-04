using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.Repositories
{
    public class APIHelperV1
    {
        //Initialize API Response Model For API Call
        public static BibleVerse.DTO.ApiResponseModel InitializeAPIResponse()
        {
            BibleVerse.DTO.ApiResponseModel newApiReponse = new DTO.ApiResponseModel();
            newApiReponse.ResponseBody = new List<string>();
            newApiReponse.ResponseErrors = new List<string>();

            return newApiReponse;
        }
    }
}
