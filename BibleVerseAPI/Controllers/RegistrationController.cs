using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BVCommon;
using BibleVerse.DTO.Repository;
using BibleVerse.DTO;
using Newtonsoft.Json;
using System.Net.Http;

namespace BibleVerseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly RegistrationRepository _repository;

        public RegistrationController(RegistrationRepository repository) => _repository = repository;

        [HttpGet]
        public IActionResult Get()
        {
            if (_repository.GetAllUsers().Count > 0)
            {
                return Ok(_repository.GetAllUsers());
            } else if(_repository.GetAllUsers().Count == 0)
            {
                return NotFound("No Users Found");
            } else
            {
                return BadRequest("Bad Request");
            }
        }

        
        [HttpPost]
        public IActionResult CreateUser([FromBody] object userRequest)
        {
            Users newUser = JsonConvert.DeserializeObject<Users>(userRequest.ToString());

            var userCreation = _repository.CreateUser(newUser);

            if(userCreation == "Success")
            {
                return Ok("User Created!");

            } else if(userCreation == "Failure")
            {
                return Conflict("An Error Occured! Try Again");
            } else
            {
                return BadRequest("Bad Request!");
            }
            
        }
        

    }
}