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


        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public async Task<ObjectResult> ConfirmEmail(EmailConfirmationModel ecom)
        {
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
