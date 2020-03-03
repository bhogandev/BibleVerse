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

namespace BibleVerse.Controllers
{
    public class Login : Controller
    {
        BibleVerseAPI _api = new BibleVerseAPI();
        public async Task<IActionResult> Index()
        {
            string users = "";
            HttpClient client = _api.Initial();
            var result = await client.GetAsync("Registration");
            
            if(result.IsSuccessStatusCode)
            {
                try
                {
                    var res = result.Content.ReadAsStringAsync().Result;
                    users = res;
                } catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return View();
        }

        public IActionResult Register()
        { 
          //Make API call to register user by passing user information      
            return View();
        }
    }
}