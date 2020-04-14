using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BibleVerseAPI.Controllers
{
    [Route("api/[controller]")]
    public class AWSController : Controller
    {
        // POST api/values
        [HttpPost]
        [Route("api/[controller]/CreateUserBuckets")]
        public void CreateUserBuckets([FromBody]string newUser)
        {
            //Run Method in aws repository to create user Buckets
        }
    }
}
