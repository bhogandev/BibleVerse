using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BibleVerse.DTO;
using BibleVerse.Helper;
using Microsoft.AspNetCore.Identity;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace BibleVerse.Controllers
{
    public class AccountController : Controller
    {

        BibleVerseAPI _api = new BibleVerseAPI();
        SignInManager<Users> signInManager;

        public AccountController(SignInManager<Users> _signInManager)
        {
            signInManager = _signInManager;
        }

        public async Task<IActionResult> Account(UserViewModel user)
        {
            if (HttpContext.Session.GetString("user") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

    }  
}
