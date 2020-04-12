using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using BibleVerse.DTO;
using Newtonsoft.Json;
using System.Net.Http;
using BibleVerse.Helper;
using System.Text;
using System.Net;

namespace BibleVerse.Controllers
{
    public class BBVController : Controller
    {

        BibleVerseAPI _api = new BibleVerseAPI();
        SignInManager<Users> signInManager;

        public BBVController(SignInManager<Users> _signInManager)
        {
            signInManager = _signInManager;
        }


        public async Task<IActionResult> Index(UserViewModel user)
        {
            if (HttpContext.Session.GetString("user") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            //Get Users Posts
            HttpClient client = _api.Initial();
            var userName = JsonConvert.DeserializeObject<Users>(HttpContext.Session.GetString("user"));
            var result = await client.GetAsync("Post?userName=" + userName.UserName);

            //Verify user was created
            var r = result.Content.ReadAsStringAsync();


            if(r.IsCompletedSuccessfully)
            {
                HttpContext.Session.SetString("posts", r.Result);
            } else
            {
                HttpContext.Session.SetString("posts", "An errr occured while retrieving your posts");
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreatePost(PostModel newPost)
        {
            if (HttpContext.Session.GetString("user") != null)
            {
                HttpClient client = _api.Initial();
                var requestBody = new StringContent(JsonConvert.SerializeObject(newPost), Encoding.UTF8, "application/json");
                var result = await client.PostAsync("Post", requestBody);

                //Verify user was created
                var r = result.Content.ReadAsStringAsync();

                if (r.IsCompletedSuccessfully)
                {

                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }


        }

        public async Task<IActionResult> Logout()
        {
            Users currUser = JsonConvert.DeserializeObject<Users>(HttpContext.Session.GetString("user"));
            UserViewModel signedInUser = new UserViewModel()
            {
                UserID = currUser.UserId,
                UserName = currUser.UserName,
                Email = currUser.Email,
                Age = currUser.Age,
                ExpPoints = currUser.ExpPoints,
                Friends = currUser.Friends,
                Level = currUser.Level,
                OnlineStatus = "Offline",
                Status = currUser.Status,
                RwdPoints = currUser.RwdPoints,
                OrganizationId = currUser.OrganizationId
            };

            if(ModelState.IsValid)
            {
                //Write Logic to update user information based on userView model at sign out time
                HttpClient client = _api.Initial();
                StringContent requestBody = new StringContent(JsonConvert.SerializeObject(signedInUser), Encoding.UTF8, "application/json");
                HttpResponseMessage result = await client.PostAsync("Logout", requestBody);
                var r = result.Content.ReadAsStringAsync();

                if(r.IsCompletedSuccessfully)
                {
                    if(result.StatusCode == HttpStatusCode.OK)
                    {
                        await signInManager.SignOutAsync();
                        return RedirectToAction("Index", "Home");
                    } else
                    {
                        TempData["Errors"] = result.Content.ReadAsStringAsync().Result;
                        await signInManager.SignOutAsync();
                        return RedirectToAction("Index", "Home");
                    }
                } else
                {
                    TempData["Errors"] = "An error occured, Please try again!";
                    await signInManager.SignOutAsync();
                    return RedirectToAction("Index", "Home");
                }
            } else
            {
                TempData["Errors"] = "User Not Currently Signed In";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
