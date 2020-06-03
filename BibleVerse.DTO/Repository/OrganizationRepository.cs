﻿using BVCommon;
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
            bool userIsMember = false;
            
            /*
             * Steps to complete:
             * [x] find user
             * [x] find organization being requested
             * [x] see if user is a member of that organization
             * [x] return org profile view with true or false of userIsMember
             */

            //Find user
             var user = from x in userManager.Users
                       where x.UserName == userName
                       select x;

            if (user.FirstOrDefault() != null)
            {
                Users u = user.First();

                //Find organization being requested
                var reqOrg = from x in _context.Organization
                             where x.OrganizationId == orgID
                             select x;

                if (reqOrg.FirstOrDefault() != null)
                {
                    Organization rOrg = reqOrg.First();

                    //See if user is a member of requested organization
                    if(rOrg.OrganizationId == u.OrganizationId)
                    {
                        userIsMember = true;
                    }

                    //Retrieve Org Posts

                    List<Posts> rOrgPosts = new List<Posts>();

                    var orgPosts = from x in _context.Posts
                                   where x.Username == rOrg.OrganizationId
                                   select x;

                    if(orgPosts.FirstOrDefault() != null)
                    {
                        rOrgPosts = orgPosts.ToList();
                    }

                    //Create Org Profile View
                    OrgProfile orgProfile = new OrgProfile()
                    {
                        Name = rOrg.Name,
                        Followers = 0, //Set to followers count
                        Following = 0, //Set to following count
                        Members = rOrg.Members,
                        Posts = rOrgPosts
                    };

                    apiResponse.ResponseMessage = "Success";
                    apiResponse.ResponseBody.Add(JsonConvert.SerializeObject(orgProfile)); //apiResponse.ResponseBody[0] = orgProfile
                    apiResponse.ResponseBody.Add(JsonConvert.SerializeObject(userIsMember)); //apiResponse.ResponseBody[1] = userIsMember bool
                }
                else
                {
                    //return error api response of org not found
                    apiResponse.ResponseMessage = "Failure";
                    apiResponse.ResponseErrors.Add("Organizaiton Not Found!");
                }
            }
            else
            {
                //return error api response of user not found
                apiResponse.ResponseMessage = "Failure";
                apiResponse.ResponseErrors.Add("Request User Not Found!");
            }

            return apiResponse;
        }

        public async Task<ApiResponseModel> GetOrgMembers(string orgID)
        {
            ApiResponseModel apiResponse = new ApiResponseModel();
            apiResponse.ResponseErrors = new List<string>();
            apiResponse.ResponseBody = new List<string>();

            var rOrgMembers = from x in userManager.Users
                              where x.OrganizationId == orgID
                              select x;

            if(rOrgMembers.FirstOrDefault() != null)
            {
                List<SearchViewModel> orgMembers = new List<SearchViewModel>();

                foreach(Users member in rOrgMembers.ToList())
                {
                    var memProfile = from x in _context.Profiles
                                     where x.ProfileId == member.UserId
                                     select x;

                    Profiles mProfile = memProfile.First();

                    SearchViewModel searchView = new SearchViewModel()
                    {
                        UserName = member.UserName,
                        PictureURL = mProfile.Picture,
                    };

                    orgMembers.Add(searchView);
                }

                apiResponse.ResponseMessage = "Success";
                apiResponse.ResponseBody.Add(JsonConvert.SerializeObject(orgMembers));
            }
            else
            {
                apiResponse.ResponseMessage = "Failure";
                apiResponse.ResponseErrors.Add("An Error Occured While Retrieving Org Members");
            }

            return apiResponse;
        }
    }
}
