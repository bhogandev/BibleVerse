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
using Microsoft.EntityFrameworkCore;
using System.Text;

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
        public async Task<IActionResult> CreateUser()
        {
            // Write Logic to Create User
            string defaultuid = "000000-000000-000000-000000";
            string username = Request.Form["Username"].ToString();
            string password = Request.Form["Password"].ToString();
            string email = Request.Form["Email"].ToString();
            string phoneNum = Request.Form["PhoneNum"].ToString();
            string organizationID = Request.Form["OrganizationId"].ToString();
            DateTime dob = DateTime.Parse(Request.Form["DOB"].ToString());
            int age = 0;

            Users newUser = new Users()
            {
                UserId = defaultuid,
                Username = username,
                Password = password,
                Email = email,
                Level = 1,
                ExpPoints = 0,
                RwdPoints = 0,
                Status = "Member",
                OnlineStatus = "Offline",
                Friends = 0,
                PhoneNum = phoneNum,
                DOB = dob,
                Age = age,
                OrganizationId = organizationID,
                isSuspended = false,
                isDeleted = false,
                ChangeDateTime = DateTime.Now,
                CreateDateTime = DateTime.Now
            };

            HttpClient client = _api.Initial();
            var requestBody = new StringContent(JsonConvert.SerializeObject(newUser), Encoding.UTF8, "application/json");
            var result = await client.PostAsync("Registration", requestBody);

            //Verify user was created
            var r = result.Content.ReadAsStringAsync();

            if (r.IsCompletedSuccessfully)
            {
                Console.WriteLine("Success");
                return View("Index");
            }
            else
            {
                Console.WriteLine("Fail");
                return View("Index");
            }
        }

    }
}
