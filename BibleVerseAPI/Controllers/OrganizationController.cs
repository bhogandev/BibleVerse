using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BibleVerse.DTO.Repository;
using BibleVerse.DTO;
using Newtonsoft.Json;

namespace BibleVerseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class OrganizationController : Controller
    {
        private readonly OrganizationRepository _repository;

        public OrganizationController(OrganizationRepository repository) => _repository = repository;

        //Get organization profile
        [HttpGet]
        [ActionName("Org")]
        public IActionResult Org(string userName, string orgID)
        {
            ApiResponseModel apiResponse = _repository.GetOrgProfile(userName, orgID).Result;

            if (apiResponse != null)
            {
                if (apiResponse.ResponseMessage == "Success")
                {
                    return Ok(apiResponse);
                }
                else
                {
                    return Conflict(apiResponse);
                }
            }
            else
            {
                //create Elog Error
                return BadRequest("An Error Occurred");
            }
        }

        //Get organization members
        [HttpGet]
        [ActionName("OrgMembers")]
        public IActionResult OrgMembers(string orgID)
        {
            ApiResponseModel apiResponse = _repository.GetOrgMembers(orgID).Result;

            if (apiResponse != null)
            {
                if (apiResponse.ResponseMessage == "Success")
                {
                    return Ok(apiResponse);
                }
                else
                {
                    return Conflict(apiResponse);
                }
            }
            else
            {
                //create Elog Error
                return BadRequest("An Error Occurred");
            }
        }
    }
}
