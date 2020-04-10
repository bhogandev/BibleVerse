﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Logging;
using BibleVerse.Models;
using BVCommon;
using BibleVerse.Helper;
using BibleVerse.DTO;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Net;
using BibleVerseDTO.Services;
using Microsoft.AspNetCore.Http;

namespace BibleVerse.Controllers
{
    public class HomeController : Controller
    {
        BibleVerseAPI _api = new BibleVerseAPI();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestModel userLogin)
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
                        Users resultUser = JsonConvert.DeserializeObject<LoginResponseModel>(result.Content.ReadAsStringAsync().Result).ResponseUser;
                        UserViewModel returnUser = new UserViewModel()
                        {
                            UserID = resultUser.UserId,
                            UserName = resultUser.UserName,
                            Email = resultUser.Email,
                            Level = resultUser.Level,
                            ExpPoints = resultUser.ExpPoints,
                            RwdPoints = resultUser.RwdPoints,
                            Status = resultUser.Status,
                            OnlineStatus = resultUser.OnlineStatus,
                            Age = resultUser.Age,
                            Friends = resultUser.Friends,
                            OrganizationId = resultUser.OrganizationId
                        };


                        //Here is where user will be directed to their account home page and basic user Information is passed to the next controller
                        HttpContext.Session.SetString("user", JsonConvert.SerializeObject(returnUser));

                        return RedirectToAction("Index", "BBV");
                    }
                    else if (result.ReasonPhrase == "Conflict") // If API call returns Conflict return Login Screen and display reason call failed
                    {
                        List<Error> errors = JsonConvert.DeserializeObject<LoginResponseModel>(result.Content.ReadAsStringAsync().Result).ResponseErrors;
                        ViewBag.Errors = errors;
                        return RedirectToAction("Login");
                    }
                    else if (result.ReasonPhrase == "BadRequest") // If API call returns BadRequest, return Login Screen and display Bad Request 
                    {
                        List<Error> errors = JsonConvert.DeserializeObject<LoginResponseModel>(result.Content.ToString()).ResponseErrors;
                        ViewBag.Errors = errors;
                        return View("Login");
                    }
                }
                else
                {
                    // Log Error
                    Console.WriteLine("Error Occured");
                    return View("Login"); // Return user to Login Screen displaying an Error has occurred
                }
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel newUser)
        {
            Users nu = new Users()
            {
                UserId = "000000-000000-000000-000000",
                UserName = newUser.UserName,
                PasswordHash = newUser.Password,
                Email = newUser.Email,
                Level = 1,
                ExpPoints = 0,
                RwdPoints = 0,
                Status = "Member",
                OnlineStatus = "Offline",
                Friends = 0,
                PhoneNumber = newUser.PhoneNumber,
                DOB = newUser.DOB,
                Age = BVFunctions.GetUserAge(newUser.DOB),
                OrganizationId = newUser.OrganizationID,
                isSuspended = false,
                isDeleted = false,
                ChangeDateTime = DateTime.Now,
                CreateDateTime = DateTime.Now
            };

            HttpClient client = _api.Initial();
            var requestBody = new StringContent(JsonConvert.SerializeObject(nu), Encoding.UTF8, "application/json");
            var result = await client.PostAsync("Registration", requestBody);

            //Verify user was created
            var r = result.Content.ReadAsStringAsync();

            if (r.IsCompletedSuccessfully)
            {
                //Check if responseMessage = success. If so, proceed. If not, return Reposne errors to user
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    RegistrationResponseModel registrationResponse = JsonConvert.DeserializeObject<RegistrationResponseModel>(result.Content.ReadAsStringAsync().Result);

                    //Send Confirmation Email using confirmation Token
                    string confirmationLink = Url.Action("ConfirmEmail", "Register", new { userid = registrationResponse.UserId, token = registrationResponse.ConfirmationToken }, protocol: HttpContext.Request.Scheme); // Generate confirmation email link
                    EmailService.Send(nu.Email, "Confirm Your Account", "Thank you for registering for BibleVerse. \n Please click the confirmation link to confirm your account and get started: " + confirmationLink);
                    return RedirectToAction("Login", "Home");
                }
                else if (result.StatusCode == HttpStatusCode.Conflict)
                {
                    var errors = JsonConvert.DeserializeObject<RegistrationResponseModel>(result.Content.ReadAsStringAsync().Result).ResponseErrors;
                    ViewBag.Errors = errors;
                    return View("Register");
                }
                else
                {
                    return View("Register");
                }
            }
            else
            {
                // Create Elog and log in Elog Table and return an errored have occured to user with error code
                return View("Register");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
