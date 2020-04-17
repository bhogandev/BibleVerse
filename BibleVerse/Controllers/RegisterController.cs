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
using System.Net;
using BibleVerseDTO.Services;

namespace BibleVerse.Controllers
{
    public class Register : Controller
    {
        BibleVerseAPI _api = new BibleVerseAPI();
        EmailService _emailService;
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
                    RegistrationResponseModel registrationResponse = JsonConvert.DeserializeObject<RegistrationResponseModel>(result.Content.ReadAsStringAsync().Result);

                    //Send Confirmation Email using confirmation Token
                    string confirmationLink = Url.Action("ConfirmEmail", "Register", new { userid = registrationResponse.UserId, token = registrationResponse.ConfirmationToken }, protocol: HttpContext.Request.Scheme); // Generate confirmation email link
                    _emailService.Send(nu.Email, "Confirm Your Account", "Thank you for registering for BibleVerse. \n Please click the confirmation link to confirm your account and get started: " + confirmationLink);
                    return RedirectToAction("Index","Login");
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

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(string userid, string token)
        {
            EmailConfirmationModel ecom = new EmailConfirmationModel()
            {
                userID = userid,
                token = token
            };

            HttpClient client = _api.Initial();
            var requestBody = new StringContent(JsonConvert.SerializeObject(ecom));
            var result = await client.PostAsync("Email", requestBody);

            if(result.StatusCode == HttpStatusCode.OK)
            {
                //Return confirmation screen
                return View();
            } else
            {
                //Return confirmation screen with failed message passed via ViewBag
                ViewBag.Errors = JsonConvert.DeserializeObject<List<IdentityError>>(result.Content.ReadAsStringAsync().Result);
                return View();
            }
            

        }
    }
}
