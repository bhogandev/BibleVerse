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
                            IFormFile file = userUp.UploadFiles.First();
                            byte[] fileBytes = new Byte[file.Length];
                            file.OpenReadStream().Read(fileBytes, 0, Int32.Parse(file.Length.ToString()));

                            using (var stream = new MemoryStream(fileBytes))
                            {
                                var userProfilePic = new PutObjectRequest
                                {
                                    BucketName = foundUser.OrganizationId.ToLower(),
                                    Key = foundUser.UserId.ToLower() + "/" + foundUser.UserId.ToLower() + "_pub" + "/" + "Images/Profile/" + userUp.UploadFiles.First().FileName,
                                    InputStream = stream,
                                    ContentType = file.ContentType,
                                    CannedACL = S3CannedACL.PublicRead
                                };

                                userUploadResponse = await _client.PutObjectAsync(userProfilePic);
                            };

                            if (userUploadResponse.HttpStatusCode == HttpStatusCode.OK)
                            {
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
