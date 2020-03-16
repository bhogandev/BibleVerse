using BVCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BibleVerse.DTO.Repository
{
    public class RegistrationRepository
    {
        private readonly BVContext _context;

        public RegistrationRepository(BVContext context)
        {
            this._context = context;

            if (_context.Users.Count() == 0)
            {
                Console.WriteLine("Index Out Of Range");
            }
        }

        public List<Users> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public string CreateUser(Users newUser)
        {
            System.Linq.IQueryable<string> newUID;
            bool idCreated = false;
            int retryTimes = 0;

            while (idCreated == false && retryTimes < 3)
            {
                var genUID = BVFunctions.CreateUserID();
                newUID = from c in _context.Users
                         where c.UserId == genUID
                         select c.UserId;

                if (newUID.FirstOrDefault() == null) // If userID is not already in DB
                {
                    newUser.UserId = genUID;
                    _context.Users.Add(newUser);// Add user
                    _context.SaveChanges();
                    // Verify User Was Created In DB Successfully

                    var nu = from c in _context.Users
                             where c.UserId == newUser.UserId
                             select c;

                    if (nu.FirstOrDefault().UserId == newUser.UserId)
                    {
                        idCreated = true;
                        return "Success";
                    }
                }

                retryTimes++;
            }

            return "Failure";
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
                           where ((u.Email == loginRequest.Email) && (u.Password == loginRequest.Password))
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