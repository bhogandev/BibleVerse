using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BibleVerse.DTO.Repository;
using BibleVerse.DTO;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BibleVerseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly RegistrationRepository _repository;

        public LoginController(RegistrationRepository repository) => _repository = repository;

       [HttpPost]
       public IActionResult LoginUser([FromBody] object userRequest)
        {

            var loginResponse = _repository.LoginUser(JsonConvert.DeserializeObject<LoginRequestModel>(userRequest.ToString()));

            if(loginResponse.ResponseStatus == "Success")
            {
                loginResponse.ResponseUser.PasswordHash = "";
                return Ok(JsonConvert.SerializeObject(loginResponse.ResponseUser));
            } else if(loginResponse.ResponseStatus == "Failed")
            {
                return Conflict("Invalid Login Credentials");
            } else
            {
                return BadRequest("An Error Occured");
            }
        }
    }
}
