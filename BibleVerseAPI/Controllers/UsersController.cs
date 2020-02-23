using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BibleVerse.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BVCommon;

namespace BibleVerseAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly BVContext _context;

        public UsersController(BVContext context) => _context = context;

        /*
        // GET: api/Users/GetAllUserIds
        [HttpGet]
        [Route("GetAllUserIds")]
        public ActionResult<string> GetAllUserIds(BibleVerse.DTO.Users newUser)
        {
            System.Linq.IQueryable<string> newUID;
            bool idCreated = false;
            int retryTimes = 0;

            while (idCreated == false && retryTimes < 3)
            {
                var genUID = BVFunctions.CreateUserID();
                newUID = from c in _context.Users
                         where c.UserId == BVFunctions.CreateUserID()
                         select c.UserId;


                if (newUID == null) // If userID is not already in DB
                {
                    newUser.UserId = newUID.ToString();
                    _context.Users.Add(newUser); // Add user

                    // Verify User Was Created In DB Successfully

                   var nu = _context.Users.Find(newUser);

                    if(nu != null)
                    {
                        idCreated = true;
                        return "Success";
                    }
                }

                retryTimes++;
            }

            return "Failure";
        }
        */

        [HttpGet]
        public IEnumerable<BibleVerse.DTO.Users> Get()
        {
            return _context.Users.ToList();
        }

    }
}