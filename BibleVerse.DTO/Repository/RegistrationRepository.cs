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

        public bool ValidateOrg(string OrgId)
        {
            IQueryable<Organization> org;

            org = from o in _context.Organization
                  where o.OrganizationId == OrgId
                  select o;

            if(org.FirstOrDefault() != null)
            {
                return true;
            } else
            {
                return false;
            }
        }

        //Create an organization
        public async Task<ApiResponseModel> CreateOrganization(Organization newOrg)
        {
            IQueryable<string> newOrgID;
            bool idCreated = false;
            bool orgExistsAlready = false;
            int retryTimes = 0;
            ApiResponseModel apiResponse = new ApiResponseModel();
            apiResponse.ResponseErrors = new List<string>();
            apiResponse.ResponseBody = new List<string>();

            //See if Organization Exists already
            var org = from c in _context.Organization
                      where c.Email == newOrg.Email
                      select c;

            if(org.FirstOrDefault() != null)
            {
                orgExistsAlready = true;
            }

            if(!orgExistsAlready) //If org doesn't exist, create org
            {
                // Create OrganizationID
                while (idCreated == false && retryTimes < 3)
                {
                    var genOrgID = BVFunctions.CreateUserID();
                    newOrgID = from c in _context.Organization
                               where c.OrganizationId == genOrgID
                               select c.OrganizationId;

                    if (newOrgID.FirstOrDefault() == null) //If generated ID isn't being used, assign it to org entity
                    {
                        IQueryable<string> newGenID;
                        bool subIdCreated = false;
                        int subRetryTimes = 0;

                        //Create SubscriptionID
                        while (!subIdCreated && retryTimes < 3)
                        {
                            var genSubID = BVFunctions.CreateUserID();
                            newGenID = from c in _context.Subscriptions
                                       where c.SubscriptionID == genSubID
                                       select c.SubscriptionID;

                            if (newGenID.FirstOrDefault() == null) //If subscription ID isn't being used, assign it to sub entity
                            {
                                newOrg.OrganizationId = genOrgID;

                                Subscriptions userSubscription = new Subscriptions()
                                {
                                    SubscriptionID = genSubID,
                                    OrganizationID = newOrg.OrganizationId,
                                    SubscriptionType = newOrg.Misc,
                                    ChangeDateTime = DateTime.Now,
                                    CreateDateTime = DateTime.Now
                                };

                                OrgSettings defaultOrgSettings = new OrgSettings()
                                {
                                    OrgSettingsId = newOrg.OrganizationId,
                                    OrganizationName = newOrg.Name,
                                    SharingEnabled = true,
                                    CalendarSharing = true,
                                    FollowersEnabled = true,
                                    MemMsgEnabled = true,
                                    OrgMsgEnabled = true,
                                    ChangeDateTime = DateTime.Now,
                                    CreateDateTime = DateTime.Now
                                };

                                newOrg.SubsciberId = userSubscription.SubscriptionID;
                                newOrg.Misc = "";
                                newOrg.CreateDateTime = DateTime.Now;
                                newOrg.ChangeDateTime = DateTime.Now;
                                newOrg.OrgSettingsId = newOrg.OrganizationId;

                                _context.OrgSettings.Add(defaultOrgSettings);
                                _context.Organization.Add(newOrg);
                                _context.Subscriptions.Add(userSubscription);
                                _context.SaveChanges();

                                var createdOrg = from c in _context.Organization
                                                 where c.OrganizationId == newOrg.OrganizationId
                                                 select c;

                                if (createdOrg.FirstOrDefault() != null) // If organization is actually created
                                {
                                    var createdSub = from c in _context.Subscriptions
                                                     where c.SubscriptionID == userSubscription.SubscriptionID
                                                     select c;

                                    if (createdSub.FirstOrDefault() != null) //if sub is actually created
                                    {
                                        bool refCodeCreated = false;
                                        int refRetryTimes = 0;

                                        while (!refCodeCreated && refRetryTimes < 3) // Create Ref Log +  Code
                                        {
                                            string refCode = BVFunctions.CreateRefCode();

                                            var createdRefCode = from c in _context.RefCodeLogs
                                                                 where c.RefCode == refCode
                                                                 select c;

                                            if (createdRefCode.FirstOrDefault() == null)  // If ref code isn't being used
                                            {
                                                RefCodeLogs newRefLog = new RefCodeLogs()
                                                {
                                                    OrganizationID = newOrg.OrganizationId,
                                                    RefCode = refCode,
                                                    RefCodeType = "Owner Referral Code",
                                                    isUsed = false,
                                                    isExpired = false,
                                                    ChangeDateTime = DateTime.Now,
                                                    CreateDateTime = DateTime.Now
                                                };

                                                _context.RefCodeLogs.Add(newRefLog); //Add  ref code to table
                                                _context.SaveChanges();

                                                //Exit Loop
                                                idCreated = true;
                                                subIdCreated = true;
                                                refCodeCreated = true;

                                                //Set api response to success and add data to pass
                                                apiResponse.ResponseMessage = "Success";
                                                apiResponse.ResponseBody.Add(genOrgID);
                                                apiResponse.ResponseBody.Add(refCode);
                                                return apiResponse;
                                            }
                                            retryTimes++;
                                        }
                                    }
                                }

                            }
                            subRetryTimes++;
                        }
                    }
                    retryTimes++;
                }
            } else
            {
                apiResponse.ResponseMessage = "Failure";
                apiResponse.ResponseErrors.Add("An organization with this email already exists.");
                return apiResponse;
            }

            apiResponse.ResponseMessage = "Failure";
            apiResponse.ResponseErrors.Add("Organization Creation Failed. Please try again.");
            return apiResponse;
        }

        //Create a User
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
                bool OrgExists = ValidateOrg(newUser.OrganizationId);

                if (OrgExists)
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

                            //Check if referal code changes  user status
                            if(newUser.Status != "Member") //If actual reference code is passed
                            {
                                var newUserStatus = from c in _context.RefCodeLogs
                                                    where c.RefCode == newUser.Status
                                                    select c;

                                if(newUserStatus.FirstOrDefault() != null) //If ref code is found
                                {
                                    var userStatus = BVFunctions.RetreiveStatusFromRefCode(newUserStatus.FirstOrDefault().RefCodeType);

                                    if(!userStatus.Contains("Error"))
                                    {
                                        newUser.Status = userStatus;
                                        newUserStatus.First().isUsed = true;
                                        newUserStatus.First().ChangeDateTime = DateTime.Now;
                                        _context.RefCodeLogs.Update(newUserStatus.FirstOrDefault());
                                        _context.SaveChanges();

                                    } else
                                    {
                                        apiResponse.ResponseMessage = "Failure";
                                        apiResponse.ResponseErrors.Add("Referral Code is Invalid");
                                        return apiResponse;
                                    }
                                } else
                                {
                                    apiResponse.ResponseMessage = "Failure";
                                    apiResponse.ResponseErrors.Add("Referral Code is Invalid");
                                    return apiResponse;
                                }
                            }

                            var res = await userManager.CreateAsync(newUser, newUser.PasswordHash);

                            if (res.Succeeded)
                            {
                                var org = from o in _context.Organization
                                      where o.OrganizationId == newUser.OrganizationId
                                      select o;

                                org.FirstOrDefault().Members++;
                                _context.Organization.Update(org.FirstOrDefault());
                                _context.SaveChanges();

                                idCreated = true;
                                apiResponse.ResponseMessage = "Success";
                                apiResponse.ConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(newUser);

                                apiResponse.UserId = newUser.Id;
                            }
                            else
                            {
                                //Check against error codes. If normal error, return to user, otherwise Log error in Elog and return generic error
                                apiResponse.ResponseMessage = "Failure";
                                foreach (IdentityError e in res.Errors.ToList())
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
                }
                else
                {
                    apiResponse.ResponseMessage = "Failure";
                    apiResponse.ResponseErrors.Add("Organization Not Found");
                }

                return apiResponse;
            } else //If user exits already
            {
                apiResponse.ResponseMessage = "Email already exists";
                apiResponse.ResponseErrors.Add("Email Already Exists");
                return apiResponse;
            }
        }

        public async Task<RegistrationResponseModel> ResendConfirmation(UserViewModel requestUser)
        {
            RegistrationResponseModel apiResponse = new RegistrationResponseModel();
            apiResponse.ResponseErrors = new List<string>();

            //Check if user actually exists
            var user = await userManager.FindByEmailAsync(requestUser.Email);

            if(user != null)
            {
                if (!user.EmailConfirmed)
                {
                    //resend user confirmation link
                    apiResponse.ResponseMessage = "Success";
                    apiResponse.ConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    apiResponse.UserId = user.Id;
                } else
                {
                    apiResponse.ResponseMessage = "Failure";
                    apiResponse.ResponseErrors.Add("User Already Confirmed");
                }
            } else
            {
                apiResponse.ResponseMessage = "Failure";
                apiResponse.ResponseErrors.Add("User Not Found!");
            }

            return apiResponse;
        }

        public async Task<EComResponseModel> ConfirmEmail(EmailConfirmationModel ecom)
        {
            var user = await userManager.FindByIdAsync(ecom.userID);
            EComResponseModel eComResponse = new EComResponseModel();
            eComResponse.ResponseErrors = new List<IdentityError>();


            if (user != null)
            {
                if(user.EmailConfirmed == false)
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
                } else
                {
                    eComResponse.ResponseStatus = "Email Confirmation Failed";
                    IdentityError error = new IdentityError()
                    {
                        Code = "EMAILALREADYCONFIRMED",
                        Description = "Email is already confirmed!"
                    };
                    eComResponse.ResponseErrors.Add(error);
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
            // Handle if user is suspended currently and handle if user is deleted or if user has not confirmed  email yet
            bool userFound = false;
            int retryTimes = 0;
            LoginResponseModel loginResponse = new LoginResponseModel();
            IQueryable<Users> currUser;
            IQueryable<Posts> iPosts;

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
                        if (currUser.FirstOrDefault().EmailConfirmed == true)
                        {
                            currUser.FirstOrDefault().OnlineStatus = "Online"; // Set user status to online
                            currUser.FirstOrDefault().ChangeDateTime = DateTime.Now;
                            //Log action in user actions table
                            var userUpdate = await userManager.UpdateAsync(currUser.FirstOrDefault());
                            if (userUpdate.Succeeded)
                            {
                                loginResponse.ResponseStatus = "Success";
                                loginResponse.ResponseUser = currUser.ToList<Users>().First();
                            }
                            else
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
                            //Return Login Failure and redirect user to confirmation screen
                            loginResponse.ResponseStatus = "Email not confirmed";
                            loginResponse.ResponseErrors = new List<Error>();
                            Error e = new Error()
                            {
                                Code = "EMAILNOTCONFIRMED",
                                Description = "Email not confirmed. Please confirm email."
                            };
                            loginResponse.ResponseErrors.Add(e);
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
                user.OnlineStatus = currUser.OnlineStatus;
                user.Level = currUser.Level;
                user.RwdPoints = currUser.RwdPoints;
                user.ExpPoints = currUser.ExpPoints;
                user.Friends = currUser.Friends;
                user.Status = currUser.Status;
                user.ChangeDateTime = DateTime.Now;

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