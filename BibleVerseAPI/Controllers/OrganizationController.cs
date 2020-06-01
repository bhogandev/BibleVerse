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
        private readonly UserActionRepository _repository;

        public OrganizationController(UserActionRepository repository) => _repository = repository;

        [HttpGet]
        [ActionName("Org")]
        public IActionResult Org(string userName, string orgID)
        {
            

            if (userPosts != null)
            {
                if (userPosts.Count > 0)
                {
                    return Ok(userPosts);
                }
                else if (userPosts.Count == 0)
                {
                    return Ok("No Posts Found");
                }
                else
                {
                    return Conflict("Unable to Retrieve Posts");
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
