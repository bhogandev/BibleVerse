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
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace BibleVerse.Controllers
{
    public class Login : Controller
    {
        BibleVerseAPI _api = new BibleVerseAPI();
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginUser()
        {
            // Write Logic to Login User
            LoginRequestModel userLogin = new LoginRequestModel()
            {
                Email = Request.Form["Email"].ToString(),
                Password = Request.Form["Password"].ToString()
            };

            HttpClient client = _api.Initial();
            var requestBody = new StringContent(JsonConvert.SerializeObject(userLogin), Encoding.UTF8, "application/json");
            var result = await client.PostAsync("Login", requestBody);
            var r = result.Content.ReadAsStringAsync();

            if(r.IsCompletedSuccessfully)
            {
                if (result.ReasonPhrase == "OK")
                {
                    Users currUser = JsonConvert.DeserializeObject<Users>(r.Result.ToString());
                    return View("Index"); // Return user titmeline view and pass current user data
                } else if(result.ReasonPhrase == "Conflict")
                {
                    return View("Index");
                } else if(result.ReasonPhrase == "BadRequest")
                {
                    return View("Index");
                }
                else
                {
                    return View("Index");
                }
            } else
            {
                Console.WriteLine("Error Occured");
                return View("Index");
            }
        }
    }
} 