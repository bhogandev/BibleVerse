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
                         where c.UserId == BVFunctions.CreateUserID()
                         select c.UserId;


                if (newUID == null) // If userID is not already in DB
                {
                    newUser.UserId = newUID.ToString();
                    _context.Users.Add(newUser); // Add user

                    // Verify User Was Created In DB Successfully

                    var nu = _context.Users.Find(newUser);

                    if (nu != null)
                    {
                        idCreated = true;
                        return "Success";
                    }
                }

                retryTimes++;
            }

            return "Failure";
        }
    }
}