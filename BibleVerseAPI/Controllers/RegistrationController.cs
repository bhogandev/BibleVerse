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
        public async Task<ObjectResult> CreateUser([FromBody] object userRequest)
        {
            Users newU = JsonConvert.DeserializeObject<Users>(userRequest.ToString());

            var apiResponse = await _repository.CreateUser(newU);

            if(apiResponse.ResponseMessage == "Success")
            {
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
        

    }
}