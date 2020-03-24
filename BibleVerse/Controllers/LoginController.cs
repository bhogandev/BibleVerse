using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BVCommon;
using BibleVerse.Helper;
using BibleVerse.DTO;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace BibleVerse.Controllers
{
    public class Login : Controller
    {
        BibleVerseAPI _api = new BibleVerseAPI();

        [HttpGet]
        public IActionResult Index()
        {
            //Write logic here to verify user is not already signed in. If so, User is redirected to dashboard

                return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginRequestModel userLogin)
        {
            if (ModelState.IsValid)
            {
                HttpClient client = _api.Initial();
                StringContent requestBody = new StringContent(JsonConvert.SerializeObject(userLogin), Encoding.UTF8, "application/json");
                HttpResponseMessage result = await client.PostAsync("Login", requestBody);
                var r = result.Content.ReadAsStringAsync();

                if (r.IsCompletedSuccessfully)
                {
                    if (result.ReasonPhrase == "OK") // If API call returns OK, redirect to User Dashboard with user information
                    {
                        TempData["currUser"] = r.Result.ToString();
                        return RedirectToAction("Index", "BBV");
                    }
                    else if (result.ReasonPhrase == "Conflict") // If API call returns Conflict return Login Screen and display reason call failed
                    {
                        string errorCode = r.Result.ToString();
                        TempData["error"] = errorCode;
                        return RedirectToAction("Index");
                    }
                    else if (result.ReasonPhrase == "BadRequest") // If API call returns BadRequest, return Login Screen and display Bad Request 
                    {
                        return View("Index", r.Result);
                    }
                }
                else
                {
                    // Log Error
                    Console.WriteLine("Error Occured");
                    return View("Index"); // Return user to Login Screen displaying an Error has occurred
                }
            }

            return View();
        }
    }
} 