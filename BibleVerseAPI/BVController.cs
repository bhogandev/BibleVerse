using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BibleVerseAPI
{
    public class BVController : Microsoft.AspNetCore.Mvc.Controller
    {
        protected string StackTrace = String.Empty;

        public BVController()
        {
        }
        
        /*
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            try
            {
                Initialize();
            }catch(ArgumentNullException nullEx)
            {
                //Log Exception In DB
            }catch(Exception ex)
            {
                //Log Exception In DB
            }
        }

        private void Initialize()
        {
            VerifyToken();
            RecordTransaction();
        }

        //Verify that call provided token and refresh token
        private void VerifyToken()
        {
            if (Request != null)
            {
                string token = string.Empty;
                string refreshToken = string.Empty;

                if ((String.IsNullOrEmpty(Request.Headers["token"]) || Request.Headers["token"] == "undefined") || (String.IsNullOrEmpty(Request.Headers["refreshToken"]) || Request.Headers["RefreshToken"] == "undefined"))
                    throw new ArgumentNullException("Null Value Provided For Token");
            }
        }

        private void RecordTransaction()
        {

            BibleVerse.Repositories.APIHelperV1 apihelper = new BibleVerse.Repositories.APIHelperV1(context);
            if (Request != null)
            {
                string device = string.Empty;
                string identity = string.Empty;
                string version = string.Empty;
                string requestBody = string.Empty;

                if (!string.IsNullOrEmpty(Request.Headers["device"]))
                    device = Request.Headers["device"];

                if (!string.IsNullOrEmpty(Request.Headers["identity"]))
                    identity = Request.Headers["identity"];

                if (!string.IsNullOrEmpty(Request.Headers["version"]))
                    version = Request.Headers["version"];

                if (Request.Body != null)
                    requestBody = Request.Body.ToString();

                BibleVerse.DTO.Transactions transaction = new BibleVerse.DTO.Transactions()
                {
                    ClientAddress = !string.IsNullOrEmpty(HttpContext.Connection.RemoteIpAddress.ToString()) ? HttpContext.Connection.RemoteIpAddress.ToString() : "",
                    ClientDevice = device,
                    ClientIdentity = identity,
                    ClientVersion = version,
                    TransactionController = Request.PathBase,
                    TransactionType = Request.Method,
                    TransactionPayload = requestBody,
                    TransactionTime = DateTime.Now,
                };


               _apihelper.RecordTransaction(transaction);
            }
        }
        */
    }
}
