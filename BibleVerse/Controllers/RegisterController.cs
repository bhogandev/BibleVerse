﻿using System;
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
using System.Net;

namespace BibleVerse.Controllers
{
    public class Register : Controller
    {
        BibleVerseAPI _api = new BibleVerseAPI();

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(RegisterViewModel newUser)
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
                    return RedirectToAction("Index","Register");
                }
                else if (result.StatusCode == HttpStatusCode.Conflict)
                {
                    var errors = JsonConvert.DeserializeObject<RegistrationResponseModel>(result.Content.ReadAsStringAsync().Result).ResponseErrors;
                    ViewBag.Errors = errors;
                    return View("Index");
                }
                else
                {
                    return View("Index");
                }
            }
            else
            {
                // Create Elog and log in Elog Table and return an errored have occured to user with error code
                return View("Index");
            }
        }

    }
}