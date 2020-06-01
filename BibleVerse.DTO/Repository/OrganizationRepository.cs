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
using Amazon.S3;
using Amazon.S3.Model;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Http.Internal;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;

namespace BibleVerse.DTO.Repository
{
    public class OrganizationRepository
    {
        private readonly BVIdentityContext _context;
        UserManager<Users> userManager;

        public OrganizationRepository(UserManager<Users> _userManager, BVIdentityContext context)
        {
            this._context = context;
            userManager = _userManager;
        }

        public async Task<ApiResponseModel> GetOrgProfile(string userName, string orgID)
        {
            ApiResponseModel apiResponse = new ApiResponseModel();
            apiResponse.ResponseErrors = new List<string>();
            apiResponse.ResponseBody = new List<string>();

            var user = from x in userManager.Users
                       where x.UserName == userName
                       select x;

            /*
             * Steps to complete:
             * [] find user
             * [] find organization being requested
             * [] find user organization
             * [] see if user is a member of that organization
             * [] return org profile view with true or false of userIsMember
             */





            if (user.FirstOrDefault() != null)
            {
                Users u = user.First();

                var org = from x in _context.Organization
                          where x.OrganizationId == u.OrganizationId
                          select x;

                

                //Create Organization Profile View
            }
            else
            {
                //return error api response of user not found
            }

        }
    }
}
