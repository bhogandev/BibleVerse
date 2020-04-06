using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BibleVerse.DTO;
using BibleVerse.DTO.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BibleVerseAPI.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class LogoutController : Controller
    {
        private readonly RegistrationRepository _repository;

        public LogoutController(RegistrationRepository repository) => _repository = repository;

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult LogoutUser([FromBody] object logOutRequest)
        {
            
            var logoutResponse = _repository.LogoutUser(JsonConvert.DeserializeObject<UserViewModel>(logOutRequest.ToString()));
            var lr = JsonConvert.SerializeObject(logoutResponse.Result);

            if (logoutResponse.IsCompletedSuccessfully)
            {
                if (logoutResponse.Result == "User Successfully Updated")
                {
                    return Ok("Success");
                }
                else if (logoutResponse.Result.Contains("Error")) //If Error is returned
                {
                    //Figure out what to do in case of failure
                    return Conflict(logoutResponse.Result); //Return Error
                }
                else
                {
                    return BadRequest("An Unknown Error Occured");
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
