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

namespace BibleVerse.DTO.Repository
{
    public class UserActionRepository
    {
        private readonly BVIdentityContext _context;
        private readonly IAmazonS3 _client;

        UserManager<Users> userManager;

        public UserActionRepository(UserManager<Users> _userManager, IAmazonS3 client, BVIdentityContext context)
        {
            this._context = context;
            this._client = client;
            userManager = _userManager;
        }

        //Get Posts for user
        public async Task<List<Posts>> GetUserPosts(string userName)
        {
            IQueryable<Posts> posts;
            List<Posts> userPosts = new List<Posts>();
            posts = from p in _context.Posts
                    where (p.Username == userName) && (p.IsDeleted != true)
                    select p;
            if (posts.Count() > 0)
            {
                foreach (Posts p in posts)
                {
                    userPosts.Add(p);
                }
            }

            return userPosts;
        }

        //Create Post For User
        public async Task<string> CreateUserPost(PostModel newPost)
        {
            IQueryable<Posts> postID;
            bool idexists = false;
            int retryTimes = 0;

            while (idexists == false && retryTimes < 3)
            {
                //Create new PostID's
                var genPostID = BVFunctions.CreateUserID();
                postID = from c in _context.Posts
                              where c.PostId == genPostID
                              select c;

                if(postID.FirstOrDefault() == null)
                {
                    Posts userPost = new Posts()
                    {
                        PostId = genPostID,
                        Username = newPost.UserName,
                        Body = newPost.Body,
                        URL = "", //URL will get generated here at some point in future
                        CreateDateTime = DateTime.Now,
                        ChangeDateTime = DateTime.Now
                    };
                    if (userPost.Body != null)
                    {
                        _context.Posts.Add(userPost);
                        _context.SaveChanges();
                    }/*
                    else
                    {
                        userPost.Body = "this was null";
                        userPost.Username = newPost.UserName;
                        await _context.Posts.AddAsync(userPost);
                        await _context.SaveChangesAsync();
                    }
                    */
                    //Verify Post was created

                    var postCheck = from c in _context.Posts
                                    where c.PostId == userPost.PostId
                                    select c;

                    if (postCheck.FirstOrDefault() != null)
                    {
                        idexists = true;
                        return "Success";
                    } else
                    {
                        retryTimes++;
                    }
                     
                }

            }

            return "Failure";
        }

        //Upload user profile pic
        public async Task<string> ChangeUserProfilePic(UserUpload userUp)
        {
            IQueryable<Photos> photoID;
            bool idexists = false;
            int retryTimes = 0;

            //Verify user is authentic and exists
            var user = from c in userManager.Users
                       where c.UserId == userUp.userID
                       select c;

            if (user.First() != null) //If valid user is found
            {
                Users foundUser = user.First();

                while (idexists == false && retryTimes < 3)
                {
                    //Create new PhotoID's
                    var genPhotoID = BVFunctions.CreateUserID();
                    photoID = from c in _context.Photos
                              where c.PhotoId == genPhotoID
                              select c;

                    if (photoID.FirstOrDefault() == null)
                    {
                        PutObjectResponse userUploadResponse = new PutObjectResponse();

                        try
                        {
                            //File needs to be uploaded to user dir

                            //convert base64 to file and upload to s3 dir
                            byte[] fbyte = Convert.FromBase64String(userUp.UploadFiles[0]);

                            //Object URL for updates based on AWS Naming Convention
                            string objectUrl = "https://" + foundUser.OrganizationId.ToLower() + ".s3.amazonaws.com/" + foundUser.UserId.ToLower() + "/" + foundUser.UserId.ToLower() + "_pub" + "/" + "Images/Profile/" + userUp.FileNames[0];

                            using (var stream = new MemoryStream(fbyte))
                            {
                                var userProfilePic = new PutObjectRequest
                                {
                                    BucketName = foundUser.OrganizationId.ToLower(),
                                    Key = foundUser.UserId.ToLower() + "/" + foundUser.UserId.ToLower() + "_pub" + "/" + "Images/Profile/" + userUp.FileNames[0],
                                    InputStream = stream,
                                    ContentType = userUp.FileTypes[0],
                                    CannedACL = S3CannedACL.PublicRead
                                };

                                userUploadResponse = await _client.PutObjectAsync(userProfilePic);
                            };

                            if (userUploadResponse.HttpStatusCode == HttpStatusCode.OK)
                            {

                                //Update Tables in DB
                                Photos newPhoto = new Photos()
                                {
                                    PhotoId = genPhotoID,
                                    URL = objectUrl,
                                    Caption = "",
                                    IsDeleted = false,
                                    Title = "",
                                    ChangeDateTime = DateTime.Now,
                                    CreateDateTime = DateTime.Now
                                };

                                _context.Photos.Add(newPhoto);
                                _context.SaveChanges();

                                var userProfile = from c in _context.Profiles
                                                  where c.ProfileId == foundUser.UserId
                                                  select c;

                                Profiles uprofile = userProfile.First();

                                UserHistory actionLog = new UserHistory()
                                {
                                    UserID = foundUser.UserId,
                                    ActionType = "UserUpload",
                                    ActionMessage = foundUser.UserName + "just changed their profile picture",
                                    Prev_Value = uprofile.Picture,
                                    Curr_Value = objectUrl,
                                    ChangeDateTime = DateTime.Now,
                                    CreateDateTime = DateTime.Now
                                };

                                _context.UserHistory.Add(actionLog);
                                _context.SaveChanges();

                                uprofile.Picture = objectUrl;
                                uprofile.ChangeDateTime = DateTime.Now;

                                _context.Profiles.Update(uprofile);
                                _context.SaveChanges();

                                idexists = true;
                                return "Success";
                            }
                            else
                            {
                                retryTimes++;
                            }
                        }catch(Exception ex)
                        {
                            return ex.ToString();
                        }
                    }

                }
            } 
            return "Failure";


        }
    }
}
