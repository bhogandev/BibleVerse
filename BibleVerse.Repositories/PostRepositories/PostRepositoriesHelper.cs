using Amazon.S3;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BibleVerse.Repositories.PostRepositories
{
    public class PostRepositoriesHelper
    {
        private static string StackTrace = "BibleVerse.Repositories -> PostRepositories -> PostRepositoriesHelper";

        #region Public Methods

        public static string CreateUserPost(BibleVerse.DTO.PostModel _newPost, BibleVerse.DTO.Users _user, string PostUID, BibleVerse.DALV2.BVIdentityContext _context)
        {
            bool hasAttachments = false;
            string entType = String.Empty;
            string entObj = String.Empty;
            bool historyWrite = false;
            bool postWrite = false;

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
                        try
                        {
                            //File needs to be uploaded to user dir

                            //convert base64 to file and upload to s3 dir
                            byte[] fbyte = Convert.FromBase64String(uUp.UploadFiles[0]);

                            //Object URL for updates based on AWS Naming Convention
                            string objectUrl = "https://" + _newPost.OrganizationId.ToLower() + ".s3.amazonaws.com/" + _newPost.UserId.ToLower() + "/" + _newPost.UserId.ToLower() + "_pub" + "/" + "Images/Photos/" + uUp.FileNames[0];

                            AmazonS3Client amazonS3Client = new AmazonS3Client();

                            //Call AWS REPO here
                            AWSRepository awsRepository = new AWSRepository(amazonS3Client, _context);

                            Task<BibleVerse.DTO.PostsRelations> uploadResult = awsRepository.uploadObject("PHOTO", fbyte, objectUrl, PostUID + "i" + _newPost.Images.IndexOf(uUp).ToString(), uUp, _newPost);

                            if(uploadResult.IsCompleted && uploadResult.Result == null)
                            {
                                BibleVerse.Exceptions.BVException exception = new Exceptions.BVException("Post Upload Resulted In Null Value", StackTrace, 03454);
                                throw exception;
                            }

                        }
                        catch (Exception ex)
                        {
                            return ex.ToString();
                        }
                    }
                }
                    //if  attachments contains videos
                    if ((_newPost.Videos != null && _newPost.Videos.Count > 0))
                    {
                        //Loop through video attachments
                        foreach (BibleVerse.DTO.UserUpload uUp in _newPost.Videos)
                        {

                            try
                            {
                                //File needs to be uploaded to user dir

                                //convert base64 to file and upload to s3 dir
                                byte[] fbyte = Convert.FromBase64String(uUp.UploadFiles[0]);

                                //Object URL for updates based on AWS Naming Convention
                                string objectUrl = "https://" + _newPost.OrganizationId.ToLower() + ".s3.amazonaws.com/" + _newPost.UserId.ToLower() + "/" + _newPost.UserId.ToLower() + "_pub" + "/" + "Videos/PostVideos/" + uUp.FileNames[0];

                                AmazonS3Client amazonS3Client = new AmazonS3Client();

                                //Call AWS REPO here
                                AWSRepository awsRepository = new AWSRepository(amazonS3Client, _context);

                                Task<BibleVerse.DTO.PostsRelations> uploadResult = awsRepository.uploadObject("VIDEO", fbyte, objectUrl, PostUID + "v" + _newPost.Images.IndexOf(uUp).ToString(), uUp, _newPost);

                                if (uploadResult.IsCompleted && uploadResult.Result == null)
                                {
                                    BibleVerse.Exceptions.BVException exception = new Exceptions.BVException("Post Upload Resulted In Null Value", StackTrace, 03454);
                                    throw exception;
                                }
                        }
                            catch (Exception ex)
                            {
                                return ex.ToString();
                            }
                        }
                    }
                    
                    // create the post
                    BibleVerse.DTO.Posts userPost = new BibleVerse.DTO.Posts()
                    {
                        PostId = PostUID,
                        Username = _newPost.UserName,
                        Body = _newPost.Body,
                        URL = "", //URL will get generated here at some point in future
                        Attachments = JsonConvert.SerializeObject(postAttachments),
                        CreateDateTime = DateTime.Now,
                        ChangeDateTime = DateTime.Now
                    };
                    if (userPost.Body != null || userPost.Attachments != null)
                    {
                     entType = userPost.GetType().Name;

                     entObj = JsonConvert.SerializeObject(userPost);

                    postWrite = BVCommon.BVContextFunctions.WriteToDb(entType, entObj, _context);
                    }
                }
                else
                {
                    // create the post  with no attachments
                    BibleVerse.DTO.Posts userPost = new BibleVerse.DTO.Posts()
                    {
                        PostId = PostUID,
                        Username = _newPost.UserName,
                        Body = _newPost.Body,
                        URL = "", //URL will get generated here at some point in future
                        CreateDateTime = DateTime.Now,
                        ChangeDateTime = DateTime.Now
                    };

                    if (userPost.Body != null || userPost.Attachments != null)
                    {
                     entType = userPost.GetType().Name;

                     entObj = JsonConvert.SerializeObject(userPost);

                     postWrite = BVCommon.BVContextFunctions.WriteToDb(entType, entObj, _context);
                }
                }

            BibleVerse.DTO.UserHistory userHistory = new DTO.UserHistory()
            {
                ActionMessage = String.Format("{0} just created a new post.", _user.UserName),
                UserID = _user.UserId,
                ChangeDateTime = DateTime.Now,
                CreateDateTime = DateTime.Now
            };

             entType = userHistory.GetType().Name;

             entObj = JsonConvert.SerializeObject(userHistory);

             historyWrite = BVCommon.BVContextFunctions.WriteToDb(entType, entObj, _context);

            return historyWrite && postWrite ? "Success" : "Failure";

        }

        public static string DeleteUserPost(BibleVerse.DTO.Posts _removablePost, BibleVerse.DTO.Users _user, BibleVerse.DALV2.BVIdentityContext _context)
        {
            if (_removablePost.Username == _user.UserName)
            {
                _removablePost.IsDeleted = true;

                string entType = _removablePost.GetType().Name;

                string entObj = JsonConvert.SerializeObject(_removablePost);

                bool postIsDeleted = BVCommon.BVContextFunctions.UpdateToDb(entType, entObj, _context);

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
