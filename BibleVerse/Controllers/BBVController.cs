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


        public IActionResult Index(UserViewModel user)
        {
            if (HttpContext.Session.GetString("user") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            //Grab user's timeline posts
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
            /*
            UserViewModel  signedInUser = JsonConvert.DeserializeObject<UserViewModel>(ViewBag.CurrUser);
            
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
                return Index(signedInUser);
            }
            */
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
