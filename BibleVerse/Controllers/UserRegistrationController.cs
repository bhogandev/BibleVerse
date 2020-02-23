using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BVCommon;
using Microsoft.EntityFrameworkCore;

namespace BibleVerse.Controllers
{
    public class UserRegistrationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            bool uidUnique = false;

            //Write Logic to register user to db
            while (!uidUnique)
            {
                //  Generate New User ID
                string newUID = BVFunctions.CreateUserID();

                
                
                // Query Users Table And See if match is found. If not: Set uidUnique = true, else let loop through again
            }
                
            return View();
        }
    }
}