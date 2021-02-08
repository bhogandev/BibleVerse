using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BVCommon;
using BibleVerse.Repositories;
using BibleVerse.DTO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Web;
using BibleVerseDTO.Services;
using System.Text;

namespace BibleVerseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RegistrationController : ControllerBase
    {
        private readonly RegistrationRepository _repository;
        private readonly AWSRepository _awsrepository;

        public RegistrationController(RegistrationRepository repository, AWSRepository aWSRepository)
        {
            this._repository = repository;
            this._awsrepository = aWSRepository;
        }

        #region Methods

        //Send Confirmation Email using confirmation Token
        private void SendConfirmationEmail(string userID, string userEmail, string confirmationToken)
        {
            string confirmationLink = Url.Action("ConfirmEmail", "Home", new { userid = userID, token = HttpUtility.UrlEncode(confirmationToken) }, protocol: HttpContext.Request.Scheme); // Generate confirmation email link
            EmailService.Send(userEmail, "Confirm Your Account", "Thank you for registering for BibleVerse. \n Please click the confirmation link to confirm your account and get started: " + confirmationLink);
        }

        #endregion

        [HttpGet]
        [ActionName("UserProfile")]
        public async Task<ObjectResult> GetUserProfile()
        {
            //Get JWT from Request Header
            var token = Request.Headers["Token"];

            //Validate Token, If not valid, send conflict with ExpiredTokenMessage


            //Pass valid token here
            ApiResponseModel response = await _repository.FUFAT(token);

            if (response.ResponseMessage == "Success")
            {
                return Ok(response);

            }
            else if (response.ResponseMessage == "Failure")
            {
                return Conflict(response);
            }
            else
            {
                // Create an ELog and Log error
                return BadRequest("An Unexpected Error Occured");
            }
        }


        [HttpGet]
        [ActionName("Search")]
        public async Task<ObjectResult> Search(string username, string user)
        {
            ApiResponseModel response = await _repository.FindUser(username, user);

            if(response.ResponseMessage == "Success")
            {
                return Ok(response);

            } else if (response.ResponseMessage == "Failure")
            {
                return Conflict(response);
            }
            else
            {
                // Create an ELog and Log error
                return BadRequest("An Unexpected Error Occured");
            }
        }

        [HttpPost]
        [ActionName("CreateUser")]
        public async Task<ObjectResult> CreateUser([FromBody] object userRequest)
        {
            Users newU = JsonConvert.DeserializeObject<Users>(userRequest.ToString());

            var apiResponse = await _repository.CreateUser(newU);

            if(apiResponse.ResponseMessage == "Success")
            {
                //Send ConfirmationEmail Token
                SendConfirmationEmail(apiResponse.User.UserId, apiResponse.User.Email, apiResponse.Misc);

                var requestBody = new StringContent(JsonConvert.SerializeObject(apiResponse.User), Encoding.UTF8, "application/json");

                var awsresult = await _awsrepository.CreateUserDir(apiResponse.User);

                if (awsresult.ResponseMessage == "Success")
                {
                    //Do something here
                }
                else
                {
                    //Log in ELog and create task
                }


                return Ok(apiResponse);

            } else if(apiResponse.ResponseMessage == "Failure")
            {
                return Conflict(apiResponse);
            } else if(apiResponse.ResponseMessage == "Email already exists")
            {
                return Conflict(apiResponse);
            } else
            {
                // Create an ELog and Log error
                return BadRequest("An Unexpected Error Occured");
            }
        }
        
        [HttpPost]
        [ActionName("CreateOrg")]
        public async Task<ObjectResult> CreateOrg([FromBody] object userOrg)
        {
            Organization newOrg = JsonConvert.DeserializeObject<Organization>(userOrg.ToString());

            var apiResponse = await _repository.CreateOrganization(newOrg);

            if(apiResponse.ResponseMessage == "Success")
            {
                return Ok(apiResponse);
            } else if(apiResponse.ResponseMessage == "Failure")
            {
                return Conflict(apiResponse);
            } else
            {
                apiResponse.ResponseMessage = "BadRequest";
                apiResponse.ResponseErrors = new List<string>();
                apiResponse.ResponseErrors.Add("An Unexpected Error Occured. Please try again");
                return BadRequest(apiResponse);
            }
        }

    }
}