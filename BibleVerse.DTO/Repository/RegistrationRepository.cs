using BVCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace BibleVerse.DTO.Repository
{
    public class RegistrationRepository
    {
        private readonly BVIdentityContext _context;

        UserManager<Users> userManager;
        
        public RegistrationRepository(UserManager<Users> _userManager , BVIdentityContext context)
        {
            userManager = _userManager;
            this._context = context;
        }

        public List<Users> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public async Task<RegistrationResponseModel> CreateUser(Users newUser)
        {
            System.Linq.IQueryable<string> newUID;
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

        public LoginResponseModel LoginUser(LoginRequestModel loginRequest)
        {
            bool userFound = false;
            int retryTimes = 0;
            LoginResponseModel loginResponse = new LoginResponseModel();
            IQueryable<Users> currUser;

            while (userFound == false && retryTimes < 3)
            {
                currUser = from u in _context.Users
                           where ((u.Email == loginRequest.Email) && (u.PasswordHash == loginRequest.Password))
                           select u;
                if (currUser.FirstOrDefault() != null)
                {
                    if (currUser.FirstOrDefault().Email == loginRequest.Email)
                    {
                        currUser.FirstOrDefault().OnlineStatus = "Online";
                        currUser.FirstOrDefault().ChangeDateTime = DateTime.Now;
                        _context.SaveChanges();
                        userFound = true;
                        loginResponse.ResponseStatus = "Success";
                        loginResponse.ResponseUser = currUser.ToList<Users>().First();
                    }
                }
                else
                {
                    retryTimes++;
                }
            }

            if(userFound)
            {
                return loginResponse;
            } else
            {
                loginResponse.ResponseStatus = "Failed";
                return loginResponse;
            }
        }
    }
}