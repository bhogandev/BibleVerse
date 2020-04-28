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

        [HttpPost]
        public async Task<IActionResult> CreateProfilePic(IFormFile profilePic)
        {
            if (HttpContext.Session.GetString("user") != null)
            {
                UserUpload userUpload = new UserUpload()
                {
                    userID = JsonConvert.DeserializeObject<Users>(HttpContext.Session.GetString("user")).UserId,
                    UploadFiles = new List<IFormFile>()
                };

                userUpload.UploadFiles.Add(profilePic);

                HttpClient client = _api.Initial();
                var requestBody = new StringContent(JsonConvert.SerializeObject(userUpload), Encoding.UTF8, "application/json");
                var result = await client.PostAsync("Post/UploadProfilePic", requestBody);
                var r = result.Content.ReadAsStringAsync();

                if (r.IsCompletedSuccessfully)
                {
                    if (result.ReasonPhrase == "OK")
                    {
                        return View("Account"); // Return user to create their user account
                    }
                    else if (result.ReasonPhrase == "Conflict")
                    {
                        return View("Account");
                    }
                    else
                    {
                        return View("Account");
                    }
                }
                else
                {
                    // Log Error in ELog
                    Console.WriteLine("Error Occured");
                    return View("ConfirmEmail"); // Return user to Login Screen displaying an Error has occurred
                }

            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
    }
}
