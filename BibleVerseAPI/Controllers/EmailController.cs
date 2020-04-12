using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BibleVerse.DTO;
using BibleVerse.DTO.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BibleVerseAPI.Controllers
{
    [Route("api/[controller]")]
    public class EmailController : Controller
    {
        private readonly RegistrationRepository _repository;

        public EmailController(RegistrationRepository repository) => _repository = repository;

        //GET 
        [HttpGet]
        public async Task<ObjectResult> Get(string userid, string token)
        {
            EmailConfirmationModel ecom = new EmailConfirmationModel()
            {
                userID = userid,
                token = token
            };

            var eComResponse = await _repository.ConfirmEmail(ecom);

            if (eComResponse.ResponseStatus == "Email Confirmed")
            {
                return Ok(eComResponse.ResponseStatus);
            }
            else
            {
                return Conflict(eComResponse.ResponseErrors);
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
