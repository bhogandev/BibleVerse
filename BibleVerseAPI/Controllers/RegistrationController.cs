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
    [Route("api/[controller]/[action]")]
    public class RegistrationController : ControllerBase
    {
        private readonly RegistrationRepository _repository;

        public RegistrationController(RegistrationRepository repository) => _repository = repository;

        [HttpGet]
        [ActionName("GetUsers")]
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