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
    [Route("api/[controller]/[action]")]
    public class AWSController : Controller
    {
        private readonly AWSRepository _repository;

        public AWSController(AWSRepository repository) => _repository = repository;

        [HttpPost]
        [ActionName("CreateUserDir")]
        public void CreateUserDir([FromBody]string newUser)
        {
            //Run Method in aws repository to create user dir in org bucket

        }

        
        [HttpPost]
        [ActionName("CreateOrgBucket")]
        public void CreateOrgBucket([FromBody] string org)
        {
            /*
            Organization newOrg = JsonConvert.DeserializeObject<Organization>(org.ToString());

            var apiResponse = await _repository.CreateOrgBucket(newOrg);

            if (apiResponse.ResponseMessage == "Success")
            {
                return Ok(apiResponse);
            }
            else if (apiResponse.ResponseMessage == "Failure")
            {
                return Conflict(apiResponse);
            }
            else
            {
                apiResponse.ResponseMessage = "BadRequest";
                apiResponse.ResponseErrors = new List<string>();
                apiResponse.ResponseErrors.Add("An Unexpected Error Occured. Please try again");
                return BadRequest(apiResponse);
            }
            */
        }
    }  
}
