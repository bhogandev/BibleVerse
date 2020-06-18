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
        private readonly RegistrationRepository _repository;
        private readonly ELogRepository _elogRepository;

        public LoginController(RegistrationRepository repository)
        {
            _repository = repository;
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
                lr = JsonConvert.SerializeObject(loginResponse.Result);

                //Create ELog Storing Exception
                var error = _elogRepository.LogError("UserLogin", 3, ex.ToString());
            }

            if (loginResponse.IsCompletedSuccessfully)
            {
                if (loginResponse.Result.ResponseStatus == "Success")
                {
                    CookieOptions cookieOptions = new CookieOptions()
                    {
                        HttpOnly = true,
                        Expires = DateTime.Now.AddDays(1)
                    };

                    Response.Cookies.Append("token", loginResponse.Result.Misc, cookieOptions);

                    return Ok();
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

        [HttpPost]
        [ActionName("RefreshToken")]
        public IActionResult RefreshToken([FromBody] object refreshRequest)
        {
            var response = _repository.AuthorizeRefreshRequest(JsonConvert.DeserializeObject<RefreshRequest>(refreshRequest.ToString()));


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
