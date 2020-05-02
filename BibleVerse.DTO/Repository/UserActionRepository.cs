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
                    orderby p.CreateDateTime descending
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
            bool hasAttachments = false;
            int retryTimes = 0;
            Posts userPost = new Posts();

            while (idexists == false && retryTimes < 3)
            {
                //Create new PostID's
                var genPostID = BVFunctions.CreateUserID();
                postID = from c in _context.Posts
                         where c.PostId == genPostID
                         select c;

                if (postID.FirstOrDefault() == null)
                {
                    //Determine if user included attachments with post
                    if ((newPost.Images != null && newPost.Images.Count > 0) || (newPost.Videos != null && newPost.Videos.Count > 0))
                    {
                        hasAttachments = true;
                    }

                    //Upload attachments and create add json to post
                    if (hasAttachments)
                    {
                        List<PostsRelations> postAttachments = new List<PostsRelations>();

                        //If attachments contains images
                        if ((newPost.Images != null && newPost.Images.Count > 0))
                        {
                            //Loop through image attachments
                            foreach (UserUpload uUp in newPost.Images)
                            {
                                IQueryable<Photos> photoID;
                                bool photoidexists = false;
                                int photoretryTimes = 0;

                                //generate photo id
                                while (photoidexists == false && photoretryTimes < 3)
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
                                            byte[] fbyte = Convert.FromBase64String(uUp.UploadFiles[0]);

                                            //Object URL for updates based on AWS Naming Convention
                                            string objectUrl = "https://" + newPost.OrganizationId.ToLower() + ".s3.amazonaws.com/" + newPost.UserId.ToLower() + "/" + newPost.UserId.ToLower() + "_pub" + "/" + "Images/Photos/" + uUp.FileNames[0];

                                            using (var stream = new MemoryStream(fbyte))
                                            {
                                                var userProfilePic = new PutObjectRequest
                                                {
                                                    BucketName = newPost.OrganizationId.ToLower(),
                                                    Key = newPost.UserId.ToLower() + "/" + newPost.UserId.ToLower() + "_pub" + "/" + "Images/Photos/" + uUp.FileNames[0],
                                                    InputStream = stream,
                                                    ContentType = uUp.FileTypes[0],
                                                    CannedACL = S3CannedACL.PublicRead
                                                };

                                                userUploadResponse = await _client.PutObjectAsync(userProfilePic);
                                            };

                                            if (userUploadResponse.HttpStatusCode == HttpStatusCode.OK)
                                            {

                                                //Check if photo already exists in photo table
                                                var photoCheck = from c in _context.Photos
                                                                 where c.URL == objectUrl
                                                                 select c;

                                                if (photoCheck.FirstOrDefault() == null)
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

                                                    photoidexists = true;

                                                    PostsRelations newRelation = new PostsRelations()
                                                    {
                                                        AttachmentID = genPhotoID,
                                                        ContentType = "Photo",
                                                        FileName = uUp.FileNames[0],
                                                        Link = objectUrl
                                                    };

                                                    //Add the attachment to the list of relations
                                                    postAttachments.Add(newRelation);

                                                }
                                            }
                                            else
                                            {
                                                photoretryTimes++;
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            return ex.ToString();
                                        }
                                    }
                                    else
                                    {
                                        photoretryTimes++;
                                    }
                                }
                            }
                        }

                        //if  attachments contains videos
                            if ((newPost.Videos != null && newPost.Videos.Count > 0))
                            {
                                //Loop through video attachments
                                foreach (UserUpload uUp in newPost.Videos)
                                {
                                    IQueryable<Videos> videoID;
                                    bool videoidexists = false;
                                    int videoretryTimes = 0;

                                    //generate video id
                                    while (videoidexists == false && videoretryTimes < 3)
                                    {
                                        //Create new VideoID's
                                        var genVideoID = BVFunctions.CreateUserID();
                                        videoID = from c in _context.Videos
                                                  where c.VideoId == genVideoID
                                                  select c;

                                        if (videoID.FirstOrDefault() == null)
                                        {
                                            PutObjectResponse userUploadResponse = new PutObjectResponse();

                                            try
                                            {
                                                //File needs to be uploaded to user dir

                                                //convert base64 to file and upload to s3 dir
                                                byte[] fbyte = Convert.FromBase64String(uUp.UploadFiles[0]);

                                                //Object URL for updates based on AWS Naming Convention
                                                string objectUrl = "https://" + newPost.OrganizationId.ToLower() + ".s3.amazonaws.com/" + newPost.UserId.ToLower() + "/" + newPost.UserId.ToLower() + "_pub" + "/" + "Videos/PostVideos/" + uUp.FileNames[0];

                                                using (var stream = new MemoryStream(fbyte))
                                                {
                                                    var userProfilePic = new PutObjectRequest
                                                    {
                                                        BucketName = newPost.OrganizationId.ToLower(),
                                                        Key = newPost.UserId.ToLower() + "/" + newPost.UserId.ToLower() + "_pub" + "/" + "Videos/PostVideos/" + uUp.FileNames[0],
                                                        InputStream = stream,
                                                        ContentType = uUp.FileTypes[0],
                                                        CannedACL = S3CannedACL.PublicRead
                                                    };

                                                    userUploadResponse = await _client.PutObjectAsync(userProfilePic);
                                                };

                                                if (userUploadResponse.HttpStatusCode == HttpStatusCode.OK)
                                                {

                                                    //Check if photo already exists in photo table
                                                    var photoCheck = from c in _context.Photos
                                                                     where c.URL == objectUrl
                                                                     select c;

                                                    if (photoCheck.FirstOrDefault() == null)
                                                    {
                                                        //Update Tables in DB
                                                        Videos newVideo = new Videos()
                                                        {
                                                            VideoId = genVideoID,
                                                            URL = objectUrl,
                                                            Caption = "",
                                                            IsDeleted = false,
                                                            Title = "",
                                                            ChangeDateTime = DateTime.Now,
                                                            CreateDateTime = DateTime.Now
                                                        };

                                                        _context.Videos.Add(newVideo);
                                                        _context.SaveChanges();

                                                        videoidexists = true;

                                                        PostsRelations newRelation = new PostsRelations()
                                                        {
                                                            AttachmentID = genVideoID,
                                                            ContentType = "Video",
                                                            FileName = uUp.FileNames[0],
                                                            Link = objectUrl
                                                        };

                                                        //Add the attachment to the list of relations
                                                        postAttachments.Add(newRelation);

                                                    }
                                                }
                                                else
                                                {
                                                    videoretryTimes++;
                                                }

                                            }
                                            catch (Exception ex)
                                            {
                                                return ex.ToString();
                                            }
                                        }
                                        else
                                        {
                                            videoretryTimes++;
                                        }
                                    }
                                }

                                //this is where has attachments should end
                            }

                        // create the post
                        userPost = new Posts()
                        {
                            PostId = genPostID,
                            Username = newPost.UserName,
                            Body = newPost.Body,
                            URL = "", //URL will get generated here at some point in future
                            Attachments = JsonConvert.SerializeObject(postAttachments),
                            CreateDateTime = DateTime.Now,
                            ChangeDateTime = DateTime.Now
                        };
                        if (userPost.Body != null || userPost.Attachments != null)
                        {
                            _context.Posts.Add(userPost);
                            _context.SaveChanges();
                        }
                    } else
                    {
                        // create the post  with no attachments
                        userPost = new Posts()
                        {
                            PostId = genPostID,
                            Username = newPost.UserName,
                            Body = newPost.Body,
                            URL = "", //URL will get generated here at some point in future
                            CreateDateTime = DateTime.Now,
                            ChangeDateTime = DateTime.Now
                        };
                        if (userPost.Body != null || userPost.Attachments != null)
                        {
                            _context.Posts.Add(userPost);
                            _context.SaveChanges();
                        }
                    }
                     //Verify Post was created

                            var postCheck = from c in _context.Posts
                                            where c.PostId == userPost.PostId
                                            select c;

                            if (postCheck.FirstOrDefault() != null)
                            {
                                idexists = true;
                                return "Success";
                            }
                        } else
                {
                    retryTimes++;
                }
                    }
            return "Failure";
            }

        //Get User Profile
        public async Task<ApiResponseModel> GetUserProfile(string userID)
        {
            IQueryable<Profiles> userProfiles;
            bool profileFound = false;
            int retryTimes = 0;
            ApiResponseModel response = new ApiResponseModel();
            response.ResponseBody = new List<string>();
            response.ResponseErrors = new List<string>();

            while(!profileFound && retryTimes < 3)
            {
                userProfiles = from c in _context.Profiles
                               where c.ProfileId == userID
                               select c;

                if(userProfiles.First() != null)
                {
                    profileFound = true;
                    response.ResponseMessage = "Success";
                    response.ResponseBody.Add(JsonConvert.SerializeObject(userProfiles.First()));
                    return response;
                } else
                {
                    retryTimes++;
                }
            }

            response.ResponseMessage = "Failure";
            response.ResponseErrors.Add("User Profile Not Found");
            return response;
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

                                //Check if photo already exists in photo table
                                var photoCheck = from c in _context.Photos
                                                 where c.URL == objectUrl
                                                 select c;

                                if (photoCheck.FirstOrDefault() == null)
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
                                } else
                                {
                                    idexists = true;
                                    return "Success";
                                }
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
