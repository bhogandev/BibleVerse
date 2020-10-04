using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using BibleVerse.DTO.Repository;
using BibleVerse.DTO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace BibleVerseAPI.Controllers
{
    //Authorization for JWT token. (Need to figure out proper flow)
    //[Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LoginController : Controller
    {
        private readonly JWTSettings _jwtSettings;
        private readonly JWTRepository _jWTRepository;
        private readonly RegistrationRepository _repository;
        private readonly ELogRepository _elogRepository;

        public LoginController(RegistrationRepository repository, ELogRepository eLogRepository)
        {
            _repository = repository;
            _elogRepository = eLogRepository;
        }
        
        [HttpPost]
        [ActionName("LoginUser")]
        public IActionResult LoginUser([FromBody] object userRequest)
        {
            string lr = "";

            var loginResponse = _repository.LoginUser(JsonConvert.DeserializeObject<LoginRequestModel>(userRequest.ToString()));

            
            try
            {

                lr = JsonConvert.SerializeObject(loginResponse.Result);
            }catch(Exception ex)
            {
                //Create ELog Storing Exception
                BibleVerse.Exceptions.UserLoginException loginException = new BibleVerse.Exceptions.UserLoginException(string.Format("Error At Application Login: {0}, StackTrace: {1}", ex.ToString(), ex.StackTrace.ToString()), 00001);

                 var exceptionResponse = _elogRepository.StoreELog(loginException.LoggedException);

                if(exceptionResponse.Result != "Success")
                {
                    loginResponse.Result.ResponseStatus = "Failed";
                }
            }

            if (loginResponse.IsCompletedSuccessfully)
            {
                if (loginResponse.Result.ResponseStatus == "Success")
                {
                    return Ok(lr);
                }
                else if (loginResponse.Result.ResponseStatus == "Failed")
                {
                    return Conflict(lr);
                }
                else if(loginResponse.Result.ResponseStatus == "Email not confirmed")
                {
                    return Conflict(lr);
                }
                else
                {
                    return BadRequest(lr);
                }
            } else
            {
                //Create an Elog error
                return BadRequest("An Error Occurred");
            }
        }

        //[EnableCors("DevPolicy")]
        [HttpPost]
        [ActionName("RefreshToken")]
        public IActionResult RefreshToken([FromBody] object refreshRequest)
        {
            var response = _jWTRepository.AuthorizeRefreshRequest(JsonConvert.DeserializeObject<RefreshRequest>(refreshRequest.ToString()));


            if (response.IsCompletedSuccessfully)
            {
                if (response.Result.ResponseMessage == "Success")
                {
                    return Ok(response.Result.ResponseBody[0]);
                }
                else if (response.Result.ResponseMessage == "Failed")
                {
                    return Conflict("An Error Has Occurred");
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                //Create an Elog error
                return BadRequest("An Error Occurred");
            }
        }
    }
}
