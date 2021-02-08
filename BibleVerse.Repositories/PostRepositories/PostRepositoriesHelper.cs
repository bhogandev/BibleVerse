using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.Repositories.PostRepositories
{
    public class PostRepositoriesHelper
    {
        private static string StackTrace = "BibleVerse.Repositories -> PostRepositories -> PostRepositoriesHelper";

        #region Public Methods
        
        public static string CreateUserPost(BibleVerse.DTO.PostModel _newPost, BibleVerse.DTO.Users _user, string PostUID)
        {
            bool hasAttachments = false;
            int i = 0;

            //Determine if user included attachments with post
            if ((_newPost.Images != null && _newPost.Images.Count > 0) || (_newPost.Videos != null && _newPost.Videos.Count > 0))
            {
                hasAttachments = true;
            }

            //Upload attachments and create add json to post
            if (hasAttachments)
            {
                List<BibleVerse.DTO.PostsRelations> postAttachments = new List<BibleVerse.DTO.PostsRelations>();

                //If attachments contains images
                if ((_newPost.Images != null && _newPost.Images.Count > 0))
                {
                    //Loop through image attachments
                    foreach (BibleVerse.DTO.UserUpload uUp in _newPost.Images)
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

                                }
                                catch (Exception ex)
                                {
                                    return ex.ToString();
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
                            var genVideoID = BVCommon.BVFunctions.CreateUserID();
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
            }
            else
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
        }

        public static string DeleteUserPost(BibleVerse.DTO.Posts _removablePost, BibleVerse.DTO.Users _user)
        {
            if (_removablePost.Username == _user.UserName)
            {
                _removablePost.IsDeleted = true;

                string entType = _removablePost.GetType().FullName;

                string entObj = JsonConvert.SerializeObject(_removablePost);

                bool postIsDeleted = BVCommon.BVContextFunctions.UpdateToDb(entType, entObj);

                return postIsDeleted ? "Success" : "Failure";
            }
            else
            {
                return "You do not have access to delete this post.";
            }
        }

        #endregion

        #region Private Methods
        #endregion
    }
}
