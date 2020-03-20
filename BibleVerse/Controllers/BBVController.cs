using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BibleVerse.DTO;
using Newtonsoft.Json;

namespace BibleVerse.Controllers
{
    public class BBVController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
           ViewBag.CurrentUser = JsonConvert.DeserializeObject<BibleVerse.DTO.Users>(TempData["currUser"].ToString());
            return View();
        }
    }
}
