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
using System.IO;
using BVCommon;

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
            var result = await client.GetAsync("Post/GetTimeline?userName=" + userName.UserName);

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

        public async Task<IActionResult> Account(UserViewModel user)
        {
            if (HttpContext.Session.GetString("user") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            //Get call to retrieve users profile

            HttpClient client = _api.Initial();
            var userName = JsonConvert.DeserializeObject<Users>(HttpContext.Session.GetString("user"));
            var result = await client.GetAsync("Post/Profile?userID=" + userName.UserName);
            var r = result.Content.ReadAsStringAsync();

            if(r.IsCompletedSuccessfully)
            {
                if (result.ReasonPhrase == "OK")
                {
                    ApiResponseModel response = JsonConvert.DeserializeObject<ApiResponseModel>(r.Result);
                    HttpContext.Session.SetString("profile", response.ResponseBody[0]);
                    HttpContext.Session.SetString("posts", response.ResponseBody[3]);

                } else
                {
                    //Send user to Error page
                    RedirectToAction("Login", "Home");
                }
            } else
            {
                RedirectToAction("Login", "Home");
            }  

            return View("Account/Account");
        }

        [HttpGet]
        public async Task<IActionResult> Search(string SearchBox)
        {
            if (HttpContext.Session.GetString("user") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            UserViewModel u = JsonConvert.DeserializeObject<UserViewModel>(HttpContext.Session.GetString("user"));

            HttpClient client = _api.Initial();
            var result = await client.GetAsync("Registration/Search?user=" + u.UserID + "&username=" + SearchBox);
            ApiResponseModel response = JsonConvert.DeserializeObject<ApiResponseModel>(result.Content.ReadAsStringAsync().Result);

            if (result.StatusCode == HttpStatusCode.OK)
            {
                List<SearchViewModel> searchResults = JsonConvert.DeserializeObject<List<SearchViewModel>>(response.ResponseBody[0]);
                ViewBag.SearchResults = searchResults;
                return View();
            }
            else if(result.StatusCode == HttpStatusCode.Conflict)
            {
                //Return search screen with failed message passed via ViewBag
                ViewBag.Errors = response.ResponseErrors;
                return View();
            } else
            {
                List<string> e = new List<string>();
                e.Add("An unexpected error occurred. Please try again");
                ViewBag.Errors = e;
                return View();
            }

        }

        [HttpGet]
        public async Task<IActionResult> Profile(string username)
        {
            if (HttpContext.Session.GetString("user") != null)
            {
                UserViewModel u = JsonConvert.DeserializeObject<UserViewModel>(HttpContext.Session.GetString("user"));

    
                    HttpClient client = _api.Initial();
                    var result = await client.GetAsync("Post/Profile?userID=" + username + "&currUserName=" + u.UserName);
                    var r = result.Content.ReadAsStringAsync();

                    ApiResponseModel response = JsonConvert.DeserializeObject<ApiResponseModel>(r.Result);

                    ViewBag.UserProfile = response.ResponseBody[0];
                    ViewBag.UserViewModel = response.ResponseBody[1];
                    ViewBag.RequestResult = response.ResponseBody[2];
                    HttpContext.Session.SetString("posts", response.ResponseBody[3]);
                    if(u.UserName.ToLower() != username.ToLower())
                    {
                        ViewBag.NotUser = true;
                    }else
                {
                    HttpContext.Session.SetString("profile", response.ResponseBody[0]);
                } 
                    return View("Account/Account");
            } else
            {
                return RedirectToAction("Login", "Home");
            }

        }

        //Send friend request from one user to another
        [HttpPost]
        public async Task<IActionResult> ProcessRelationshipRequest(string SecondUser, string RequestType, string RelationShipType)
        {
            if (HttpContext.Session.GetString("user") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            UserViewModel firstUser = JsonConvert.DeserializeObject<UserViewModel>(HttpContext.Session.GetString("user"));

            RelationshipRequestModel relationshipRequest = new RelationshipRequestModel()
            {
                FirstUser = firstUser.UserName,
                SecondUser = SecondUser,
                RequestType = RequestType,
                RelationshipType = BVCommon.BVFunctions.RetrieveRelationshipType(RequestType)
            };

                HttpClient client = _api.Initial();
                var requestBody = new StringContent(JsonConvert.SerializeObject(relationshipRequest), Encoding.UTF8, "application/json");
                HttpResponseMessage result = await client.PostAsync("Post/RelationshipReq", requestBody);
                var r = result.Content.ReadAsStringAsync();

                if (r.IsCompletedSuccessfully)
                {
                    ApiResponseModel response = JsonConvert.DeserializeObject<ApiResponseModel>(r.Result);

                    if (result.ReasonPhrase == "OK")
                    {
                        return RedirectToAction("Profile", new { username = SecondUser }); // Return user to create their user account
                    }
                    else if (result.ReasonPhrase == "Conflict")
                    {
                        return RedirectToAction("Profile", new { username = SecondUser });
                    }
                    else
                    {
                        return RedirectToAction("Profile", new { username = SecondUser });
                    }
                }
                else
                {
                    // Log Error in ELog
                    Console.WriteLine("Error Occured");
                    return View("ConfirmEmail"); // Return user to Login Screen displaying an Error has occurred
                }
        }

        //Change User Profile Picture
        [HttpPost]
        public async Task<IActionResult> ChangeProfilePic(IFormFile profilePic)
        {
            if (HttpContext.Session.GetString("user") != null)
            {
                UserUpload userUpload = new UserUpload()
                {
                    userID = JsonConvert.DeserializeObject<Users>(HttpContext.Session.GetString("user")).UserId,
                    UploadFiles = new List<string>(),
                    FileNames = new List<string>(),
                    FileTypes = new List<string>()
                };

                userUpload.FileNames.Add(profilePic.FileName);
                userUpload.FileTypes.Add(profilePic.ContentType);

                using (var ms = new MemoryStream())
                {
                    profilePic.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    string s = Convert.ToBase64String(fileBytes);
                    userUpload.UploadFiles.Add(s);
                }
                    
                HttpClient client = _api.Initial();
                var requestBody = new StringContent(JsonConvert.SerializeObject(userUpload), Encoding.UTF8, "application/json");
                var result = await client.PostAsync("Post/UploadProfilePic", requestBody);
                var r = result.Content.ReadAsStringAsync();

                if (r.IsCompletedSuccessfully)
                {
                    if (result.ReasonPhrase == "OK")
                    {
                        return RedirectToAction("Account"); // Return user to create their user account
                    }
                    else if (result.ReasonPhrase == "Conflict")
                    {
                        return RedirectToAction("Account");
                    }
                    else
                    {
                        return RedirectToAction("Account");
                    }
                }
                else
                {
                    // Log Error in ELog
                    Console.WriteLine("Error Occured");
                    return View("ConfirmEmail"); // Return user to Login Screen displaying an Error has occurred
                }

            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        //Create Post On Timeline
        [HttpPost]
        public async Task<IActionResult> CreatePost(List<IFormFile> IFiles, PostModel newPost)
        {
            if (HttpContext.Session.GetString("user") != null)
            {
                UserViewModel user = JsonConvert.DeserializeObject<UserViewModel>(HttpContext.Session.GetString("user"));

                newPost.UserId = user.UserID;
                newPost.UserName = user.UserName;
                newPost.OrganizationId = user.OrganizationId;

                if(IFiles != null)
                {
                    newPost.Images = new List<UserUpload>();

                    //convert each file to base 64 and create userUpload object
                    foreach(IFormFile file in IFiles)
                    {

                        UserUpload userUpload = new UserUpload()
                        {
                            userID = newPost.UserId,
                            UploadFiles = new List<string>(),
                            FileNames = new List<string>(),
                            FileTypes = new List<string>()
                        };

                        userUpload.FileNames.Add(file.FileName);
                        userUpload.FileTypes.Add(file.ContentType);

                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            var fileBytes = ms.ToArray();
                            string s = Convert.ToBase64String(fileBytes);
                            userUpload.UploadFiles.Add(s);
                        }

                        newPost.Images.Add(userUpload);
                    }

                }

                /*
                if (IVideos != null)
                {
                    newPost.Videos = new List<UserUpload>();

                    //convert each file to base 64 and create userUpload object
                    foreach (IFormFile video in IVideos)
                    {
                        UserUpload userUpload = new UserUpload()
                        {
                            userID = newPost.UserID,
                            UploadFiles = new List<string>(),
                            FileNames = new List<string>(),
                            FileTypes = new List<string>()
                        };

                        userUpload.FileNames.Add(video.FileName);
                        userUpload.FileTypes.Add(video.ContentType);

                        using (var ms = new MemoryStream())
                        {
                            video.CopyTo(ms);
                            var fileBytes = ms.ToArray();
                            string s = Convert.ToBase64String(fileBytes);
                            userUpload.UploadFiles.Add(s);
                        }

                        newPost.Videos.Add(userUpload);
                    }
                    

                }
                */


                HttpClient client = _api.Initial();
                var requestBody = new StringContent(JsonConvert.SerializeObject(newPost), Encoding.UTF8, "application/json");
                var result = await client.PostAsync("Post/CreatePost", requestBody);

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

        //Bring user to organiation base page
        public async Task<IActionResult> Home(string orgID)
        {
            if (HttpContext.Session.GetString("user") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            //Get call to retrieve organization profile

            HttpClient client = _api.Initial();
            var user = JsonConvert.DeserializeObject<Users>(HttpContext.Session.GetString("user"));
            var result = await client.GetAsync("Organization/Org?userName=" + user.UserName + "&orgID=" + orgID);
            var r = result.Content.ReadAsStringAsync();

            if (r.IsCompletedSuccessfully)
            {
                if (result.ReasonPhrase == "OK")
                {
                    ApiResponseModel response = JsonConvert.DeserializeObject<ApiResponseModel>(r.Result);
                    HttpContext.Session.SetString("orgProfile", response.ResponseBody[0]);
                    ViewBag.UserIsMember = response.ResponseBody[1];
                }
                else
                {
                    //Send user to Error page
                    RedirectToAction("Login", "Home");
                }
            }
            else
            {
                RedirectToAction("Login", "Home");
            }

            return View("Org/Home");

        }

        //Log User Out
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
