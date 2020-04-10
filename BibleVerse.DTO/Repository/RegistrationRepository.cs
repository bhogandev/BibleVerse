using BVCommon;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Text;
using System.Security.Policy;
using System.Net.Http;

namespace BibleVerse.DTO.Repository
{
    public class RegistrationRepository
    {
        private readonly BVIdentityContext _context;
        
        UserManager<Users> userManager;
        SignInManager<Users> signInManager;
        
        public RegistrationRepository(UserManager<Users> _userManager , SignInManager<Users> _signInManager ,BVIdentityContext context)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            this._context = context;
        }

        public List<Users> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public async Task<RegistrationResponseModel> CreateUser(Users newUser)
        {
            IQueryable<string> newUID;
            bool idCreated = false;
            bool userExistsAlready = false;
            int retryTimes = 0;
            RegistrationResponseModel apiResponse = new RegistrationResponseModel();
            apiResponse.ResponseErrors = new List<string>();

            var userWEmail = from c in userManager.Users
                             where c.Email == newUser.Email
                             select c;

            if(userWEmail.FirstOrDefault() != null)
            {
                userExistsAlready = true;
            }

            if (!userExistsAlready)// If user doesn't exist already
            {
                while (idCreated == false && retryTimes < 3)
                {
                    var genUID = BVFunctions.CreateUserID();
                    newUID = from c in userManager.Users
                             where c.UserId == genUID
                             select c.UserId;

                    if (newUID.FirstOrDefault() == null) // If userID is not already in DB
                    {
                        newUser.UserId = genUID;
                        var res = await userManager.CreateAsync(newUser, newUser.PasswordHash);
         
                        if (res.Succeeded)
                        {
                            idCreated = true;
                            apiResponse.ResponseMessage = "Success";
                            apiResponse.ConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(newUser);
                            apiResponse.UserId = newUser.Id;
                        }
                        else
                        {
                            //Check against error codes. If normal error, return to user, otherwise Log error in Elog and return generic error
                            apiResponse.ResponseMessage = "Failure";
                            foreach(IdentityError e in res.Errors.ToList())
                            {
                                apiResponse.ResponseErrors.Add(e.Description);
                            }
                            
                        }

                        return apiResponse;
                    }

                    retryTimes++;
                }
                apiResponse.ResponseMessage = "Retry Timeout Failure";
                return apiResponse;
            } else //If user exits already
            {
                apiResponse.ResponseMessage = "Email already exists";
                apiResponse.ResponseErrors.Add("Email Already Exists");
                return apiResponse;
            }
        }

        public async Task<EComResponseModel> ConfirmEmail(EmailConfirmationModel ecom)
        {
            var user = await userManager.FindByIdAsync(ecom.userID);
            EComResponseModel eComResponse = new EComResponseModel();
            eComResponse.ResponseErrors = new List<IdentityError>();


            if (user != null)
            {
                var emailConfirmation = await userManager.ConfirmEmailAsync(user, ecom.token);

                if (emailConfirmation.Succeeded)
                {
                    eComResponse.ResponseStatus = "Email Confirmed";
                }
                else
                {
                    eComResponse.ResponseStatus = "Email Confirmation Failed";
                    foreach (IdentityError e in emailConfirmation.Errors)
                    {
                        eComResponse.ResponseErrors.Add(e);
                    }
                }
                return eComResponse;
            } else
            {
                eComResponse.ResponseStatus = "An Error Occurred";
                IdentityError error = new IdentityError()
                {
                    Code = "GENERROR",
                    Description = "User Not Found On Request For Confirmation"
                };
                eComResponse.ResponseErrors.Add(error);
                return eComResponse;
            }
        }

        public async Task<LoginResponseModel> LoginUser(LoginRequestModel loginRequest)
        {
            bool userFound = false;
            int retryTimes = 0;
            LoginResponseModel loginResponse = new LoginResponseModel();
            IQueryable<Users> currUser; 

            while (userFound == false && retryTimes < 3)
            {
                currUser = from u in userManager.Users
                           where u.Email == loginRequest.Email
                           select u;
                if (currUser.FirstOrDefault() != null)
                {
                   userFound = true;
                   var res = await signInManager.PasswordSignInAsync(currUser.FirstOrDefault().UserName, loginRequest.Password, true, true);

                    if(res.Succeeded)
                    {
                        currUser.FirstOrDefault().OnlineStatus = "Online";
                        currUser.FirstOrDefault().ChangeDateTime = DateTime.Now;
                        //Log action in user actions table
                       var userUpdate = await userManager.UpdateAsync(currUser.FirstOrDefault());
                        if (userUpdate.Succeeded)
                        {
                            loginResponse.ResponseStatus = "Success";
                            loginResponse.ResponseUser = currUser.ToList<Users>().First();
                        } else
                        {
                            Error error = new Error()
                            {
                                Code = "ERRORONUSERUPDATE",
                                Description = "Error updating user after successful login"
                            };
                            loginResponse.ResponseStatus = "Failed";
                            loginResponse.ResponseErrors = new List<Error>();
                            loginResponse.ResponseErrors.Add(error);
                        }
                    } else
                    {
                        List<string> identityError = ELogFunctions.GetSignInError(res);
                        Error error = new Error()
                        {
                            Code = identityError[0],
                            Description = identityError[1]
                        };
                        loginResponse.ResponseStatus = "Failed";
                        loginResponse.ResponseErrors = new List<Error>();
                        loginResponse.ResponseErrors.Add(error);
                    }
                }
                else
                {
                    retryTimes++;
                }
            }
                return loginResponse;
        }

        public async Task<string> LogoutUser(UserViewModel currUser)
        {
            //Write Logic here to sign user out
            var user = await userManager.FindByEmailAsync(currUser.Email);

            if (user != null)
            {
                currUser.OnlineStatus = "Offline";

                user.OnlineStatus = "Offline";
                user.Level = currUser.Level;
                user.RwdPoints = currUser.RwdPoints;
                user.ExpPoints = currUser.ExpPoints;
                user.Friends = currUser.Friends;

                var result = await userManager.UpdateAsync(user); //Update user with final view model at time of signout

                if (result.Succeeded)
                {
                    return "User Successfully Updated";
                }
                else
                {
                    //Log in ELog
                    return "Error: User Final Update Not Completed Upon Logout";
                }
            } else
            {
                //Log in ELog
                return "Error: User turned up NULL";
            }
        }
    }
}