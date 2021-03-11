using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.Repositories
{
    public class APIHelperV1
    {
        private readonly BibleVerse.DALV2.BVIdentityContext _context;

        public APIHelperV1(BibleVerse.DALV2.BVIdentityContext context)
        {
            this._context = context;
        }

        //Initialize API Response Model For API Call
        public static BibleVerse.DTO.ApiResponseModel InitializeAPIResponse()
        {
            BibleVerse.DTO.ApiResponseModel newApiReponse = new DTO.ApiResponseModel();
            newApiReponse.ResponseBody = new List<string>();
            newApiReponse.ResponseErrors = new List<string>();

            return newApiReponse;
        }

        public static string RetreieveResponseMessage(ResponseMessageEnum response)
        {
            switch(response){
                case ResponseMessageEnum.Success:
                    return "SUCCESS";

                case ResponseMessageEnum.Failure:
                    return "FAILURE";

                case ResponseMessageEnum.Conflict:
                    return "CONFLICT";

                default:
                    return "BADREQUEST";
            }
        }

        public enum ResponseMessageEnum
        {
            Success,
            Failure,
            Conflict
        }



        public bool RecordTransaction(BibleVerse.DTO.Transactions transaction)
        {
            string entType = transaction.GetType().Name;

            string entObject = JsonConvert.SerializeObject(transaction);

            bool result = BVCommon.BVContextFunctions.WriteToDb(entType, entObject, _context);

            return result;
        }
    }
}
