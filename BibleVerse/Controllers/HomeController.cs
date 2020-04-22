using System;
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
using System.Web;

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

        #region Initial  Views
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

        public IActionResult RegisterOrg()
        {
            ViewBag.OrgSetup = true;
            return View("Register");
        }

        public IActionResult ConfrimEmail()
        {
            return View();
        }
        #endregion

        #region Complex Views & Actions
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userid, string token)
        {
            if (userid != null && token != null)
            {
                EmailConfirmationModel ecom = new EmailConfirmationModel()
                {
                    userID = userid,
                    token = token
                };

                HttpClient client = _api.Initial();
                var requestBody = new StringContent(JsonConvert.SerializeObject(ecom));
                var result = await client.GetAsync("Email/?userid=" + ecom.userID + "&token=" + ecom.token);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    //Return confirmation screen
                    return View();
                }
                else
                {
                    //Return confirmation screen with failed message passed via ViewBag
                    ViewBag.Errors = JsonConvert.DeserializeObject<List<IdentityError>>(result.Content.ReadAsStringAsync().Result.ToString());
                    return View();
                }
            } else
            {
                ViewBag.ResendConfirmation = true;
                return View();
            }

        }

        [HttpPost]
        public async Task<IActionResult> RegisterOrg(RegisterViewModel orgBody)
        {
            Organization newOrg = new Organization()
            {
                Name = orgBody.Name,
                Email = orgBody.Email,
                Description = orgBody.Description,
                PhoneNum = orgBody.PhoneNumber,
                Misc = orgBody.SubscriptionPlan.ToString()
            };

            HttpClient client = _api.Initial();
            StringContent requestBody = new StringContent(JsonConvert.SerializeObject(newOrg), Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.PostAsync("Registration/CreateOrg", requestBody);
            var r = result.Content.ReadAsStringAsync();
            ApiResponseModel response = JsonConvert.DeserializeObject<ApiResponseModel>(r.Result);

            if (r.IsCompletedSuccessfully)
            {
                if(result.ReasonPhrase == "OK")
                {
                    newOrg.OrganizationId = response.ResponseBody[0];

                    //Create Bucket for organization
                    requestBody = new StringContent(JsonConvert.SerializeObject(newOrg), Encoding.UTF8, "application/json");
                    HttpResponseMessage awsres = await client.PostAsync("AWS/CreateOrgBucket", requestBody);
                    var awsr = awsres.Content.ReadAsStringAsync();

                    if(awsr.IsCompletedSuccessfully)
                    {

                    } else
                    {
                       //Create Elog Error and Support Task
                       
                    }

                    //Create Email to send to organization and provide owner with owner referal code and instructions
                    string refCode = response.ResponseBody[1];
                    EmailService.Send(newOrg.Email, "Your Organization Credentials", "Thank you for registering your organization with BibleVerse. \n Your organization ID is:  \n"  + response.ResponseBody[0] + "Please use the refferal code below to create your organization owner account: \n" + refCode);

                    ViewBag.OrgCode = response.ResponseBody[0];
                    return View("Register"); // Return user to create their user account
                } else if(result.ReasonPhrase == "Conflict")
                {
                    ViewBag.Errors = response.ResponseErrors;
                    ViewBag.OrgSetup = true;
                    return View("RegisterOrg");
                } else
                {
                    ViewBag.OrgSetup = true;
                    ViewBag.Errors = response.ResponseErrors;
                    return View("RegisterOrg");
                }  
            } else
            {
                // Log Error in ELog
                Console.WriteLine("Error Occured");
                return View("ConfirmEmail"); // Return user to Login Screen displaying an Error has occurred
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResendConfirmation(string userEmail)
        {
            UserViewModel requestUser = new UserViewModel()
            {
                Email = userEmail
            };

            HttpClient client = _api.Initial();
            StringContent requestBody = new StringContent(JsonConvert.SerializeObject(requestUser), Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.PostAsync("Email", requestBody);
            var r = result.Content.ReadAsStringAsync();

            if(r.IsCompletedSuccessfully)
            {
                if(result.ReasonPhrase == "OK")
                {
                    RegistrationResponseModel registrationResponse = JsonConvert.DeserializeObject<RegistrationResponseModel>(result.Content.ReadAsStringAsync().Result);

                    //Send Confirmation Email using confirmation Token
                    string confirmationLink = Url.Action("ConfirmEmail", "Home", new { userid = registrationResponse.UserId, token = HttpUtility.UrlEncode(registrationResponse.ConfirmationToken) }, protocol: HttpContext.Request.Scheme); // Generate confirmation email link
                    EmailService.Send(userEmail, "Confirm Your Account", "Thank you for registering for BibleVerse. \n Please click the confirmation link to confirm your account and get started: " + confirmationLink);
                    return View("ConfirmEmail");

                } else if(result.ReasonPhrase == "Conflict")
                {
                    List<string> errors = JsonConvert.DeserializeObject<RegistrationResponseModel>(result.Content.ReadAsStringAsync().Result).ResponseErrors;
                    ViewBag.Errors = errors;
                    ViewBag.ResendConfirmation = true;
                    return View("ConfirmEmail");
                } else
                {
                    Error e = new Error()
                    {
                        Code = "BADREQUESTERROR",
                        Description = "An Unexpected Error Occured, Please Try Again"
                    };
                    List<Error> errors = new List<Error>();
                    errors.Add(e);
                    ViewBag.Errors = errors;
                    return View("ConfirmEmail");
                }
            } else
            {
                // Log Error in ELog
                Console.WriteLine("Error Occured");
                return View("ConfirmEmail"); // Return user to Login Screen displaying an Error has occurred
            }

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
                        List<Posts> initalPosts = JsonConvert.DeserializeObject<LoginResponseModel>(result.Content.ReadAsStringAsync().Result).InitialPosts;
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
                        HttpContext.Session.SetString("posts", JsonConvert.SerializeObject(initalPosts));
                        return RedirectToAction("Index", "BBV");
                    }
                    else if (result.ReasonPhrase == "Conflict") // If API call returns Conflict return Login Screen and display reason call failed
                    {
                        List<Error> errors = JsonConvert.DeserializeObject<LoginResponseModel>(result.Content.ReadAsStringAsync().Result).ResponseErrors;
                        ViewBag.Errors = errors;
                        return View();
                    }
                    else if (result.ReasonPhrase == "BadRequest") // If API call returns BadRequest, return Login Screen and display Bad Request 
                    {
                        List<Error> errors = JsonConvert.DeserializeObject<LoginResponseModel>(result.Content.ToString()).ResponseErrors;
                        ViewBag.Errors = errors;
                        return View();
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
            if(newUser.ReferenceCode == null)
            {
                newUser.ReferenceCode = "Member";
            }

            Users nu = new Users()
            {
                UserId = "000000-000000-000000-000000",
                UserName = newUser.UserName,
                PasswordHash = newUser.Password,
                Email = newUser.Email,
                Level = 1,
                ExpPoints = 0,
                RwdPoints = 0,
                Status = newUser.ReferenceCode,
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
            var result = await client.PostAsync("Registration/CreateUser", requestBody); // Avoid locks!

            //Verify user was created
            var r = result.Content.ReadAsStringAsync();

            if (r.IsCompletedSuccessfully)
            {
                //Check if responseMessage = success. If so, proceed. If not, return Reposne errors to user
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    ApiResponseModel registrationResponse = JsonConvert.DeserializeObject<ApiResponseModel>(result.Content.ReadAsStringAsync().Result);
                    Users addedUser = registrationResponse.User;

                    //Send Confirmation Email using confirmation Token
                    string confirmationLink = Url.Action("ConfirmEmail", "Home", new { userid = addedUser.UserId , token = HttpUtility.UrlEncode(registrationResponse.Misc) }, protocol: HttpContext.Request.Scheme); // Generate confirmation email link
                    EmailService.Send(nu.Email, "Confirm Your Account", "Thank you for registering for BibleVerse. \n Please click the confirmation link to confirm your account and get started: " + confirmationLink);

                    //Create AWS Dir For User Storage
                    requestBody = new StringContent(JsonConvert.SerializeObject(addedUser), Encoding.UTF8, "application/json");
                    var awsresult = await client.PostAsync("AWS/CreateUserDir", requestBody);

                    if(awsresult.StatusCode == HttpStatusCode.OK)
                    {
                        //Do something here
                    } else
                    {
                        //Log in ELog and create task
                    }

                    return RedirectToAction("Login", "Home");
                }
                else if (result.StatusCode == HttpStatusCode.Conflict)
                {
                    var errors = JsonConvert.DeserializeObject<ApiResponseModel>(result.Content.ReadAsStringAsync().Result).ResponseErrors;
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
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
