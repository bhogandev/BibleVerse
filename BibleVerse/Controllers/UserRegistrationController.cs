using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
            //Write Logic to register user to db
             
                
            return View();
        }
    }
}