using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BibleVerse.DTO;
using BibleVerse.DTO.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BibleVerseAPI.Controllers
{
    [Route("api/[controller]")]
    public class EmailController : Controller
    {
        private readonly RegistrationRepository _repository;

        public EmailController(RegistrationRepository repository) => _repository = repository;

        //GET 
        [HttpGet]
        public async Task<ObjectResult> Get(string userid, string token)
        {
            EmailConfirmationModel ecom = new EmailConfirmationModel()
            {
                userID = userid,
                token = token
            };

            var eComResponse = await _repository.ConfirmEmail(ecom);

            if (eComResponse.ResponseStatus == "Email Confirmed")
            {
                return Ok(eComResponse.ResponseStatus);
            }
            else
            {
                return Conflict(eComResponse.ResponseErrors);
            }
        }

        [HttpPost]
        public async Task<ObjectResult> ResendConfirmation([FromBody] object ru)
        {
            UserViewModel requestUser = JsonConvert.DeserializeObject<UserViewModel>(ru.ToString());

            var response = await _repository.ResendConfirmation(requestUser);

                if(response.ResponseMessage == "Success")
                {
                    return Ok(response);
                } else if(response.ResponseMessage ==  "Failure")
                {
                    return Conflict(response);
                } else
                {
                    RegistrationResponseModel genModel = new RegistrationResponseModel()
                    {
                        ResponseMessage = "An error occurred during reconfirmation"
                    };
                    return BadRequest(response);
                }
        }
    }
}
