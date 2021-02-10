using Amazon.S3;
using Amazon.S3.Model;
using BibleVerse.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BibleVerse.Repositories.UserRepositories
{
    public class UserActionRepository
    {
        private readonly BibleVerse.DALV2.BVIdentityContext _context;
        private readonly IAmazonS3 _client;
        private readonly JWTSettings _jwtSettings;
        private readonly BibleVerse.Repositories. JWTRepository _jwtrepository;
        protected string StackTraceRoot = "BibleVerse.DTO -> Repository -> UserActionRepository: ";

        UserManager<Users> userManager;

        public UserActionRepository(UserManager<Users> _userManager, IAmazonS3 client, BibleVerse.DALV2.BVIdentityContext context, IOptions<JWTSettings> jwtSettings, JWTRepository jwtrepository)
        {
            this._context = context;
            this._client = client;
            userManager = _userManager;
            _jwtSettings = jwtSettings.Value;
            _jwtrepository = jwtrepository;
        }

        public ApiResponseModel GetUserOrg(string orgID)
        {
            ApiResponseModel response = APIHelperV1.InitializeAPIResponse();
            try
            {
                var userOrg = from x in _context.Organization
                              where x.OrganizationId == orgID
                              select x;

                if (userOrg.FirstOrDefault() != null)
                {
                    response.ResponseMessage = "Success";
                    response.ResponseBody.Add(JsonConvert.SerializeObject(userOrg.FirstOrDefault()));
                }
                else
                {
                    response.ResponseMessage = "Failure";
                    response.ResponseErrors.Add("Organization Not Found!");
                }

                return response;
            }catch(Exception ex)
            {
                try
                {
                    BibleVerse.Exceptions.BVException x = new Exceptions.BVException(String.Format("Error: \n Root: {0} \n Message: {1}", StackTraceRoot, ex.Message));
                }catch(Exception e)
                {
                    EventLog.WriteEntry("BibleVerse.BVExceptionData", e.Message);
                }

                response.ResponseMessage = "Failure";
                response.ResponseErrors.Add("An Error Occurred");
                return response;
            }
        }

        //Get Post
        public async Task<Posts> GetPost(string postID)
        {
            var pList = from p in _context.Posts
                        where p.PostId == postID
                        select p;

            if(pList.FirstOrDefault() != null)
            {
                return pList.First();
            }


            return null;
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

        public async Task<List<Posts>> GenerateTimelinePosts(string token, string refreshToken)
        {
            IQueryable<Posts> posts;
            List<Posts> userPosts = new List<Posts>();
            List<string> friends = new List<string>();
            

            RefreshRequest r = new RefreshRequest() { AccessToken = token, RefreshToken = refreshToken }; 

            //Get User From Token
            var user = _jwtrepository.FindUserFromAccessToken(r);

            var userName = user.UserName;

            var friendList = from f in _context.UserRelationships
                             where (((f.FirstUser == userName) || (f.SecondUser == userName)) && ((f.FirstUserConfirmed == true) && (f.SecondUserConfirmed == true)) && f.RelationshipType == "FRIEND")
                             select f;

            if(friendList.FirstOrDefault() != null)
            {
                foreach(UserRelationships relation  in friendList)
                {
                    if(relation.FirstUser != userName)
                    {
                        friends.Add(relation.FirstUser);
                    }else if(relation.SecondUser != userName)
                    {
                        friends.Add(relation.SecondUser);
                    }
                }
            }

            posts = from p in _context.Posts
                    where (((p.Username == userName) && (p.IsDeleted != true)) || (friends.Contains(p.Username) && (p.IsDeleted != true)))
                    orderby p.CreateDateTime descending
                    select p;

            if (posts.Count() > 0)
            {
                foreach (Posts p in posts)
                {
                    p.LikeStatus = GetLikeStatus(user, p.PostId);

                    var commentsExt = from pc in _context.Comments
                                      where pc.ParentId == p.PostId
                                      select pc;
                    if(commentsExt != null)
                    {
                        var postComments = commentsExt.ToList();
                        p.CommentsExt = JsonConvert.SerializeObject(postComments);
                    }

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

            try
            {
                RefreshRequest r = new RefreshRequest();
                r.AccessToken = newPost.UserId;

                //Find User
                var user = _jwtrepository.FindUserFromAccessToken(r);
                newPost.UserName = user.UserName;
                newPost.UserId = user.UserId;
                newPost.OrganizationId = user.OrganizationId;

                while (idexists == false && retryTimes < 3)
                {
                    //Create new PostID's
                    var genPostID = BVCommon.BVFunctions.CreateUserID();
                    postID = from c in _context.Posts
                             where c.PostId == genPostID
                             select c;

                    if (postID.FirstOrDefault() == null)
                    {
                        //Create the user post
                        string createPost = PostRepositories.PostRepositoriesHelper.CreateUserPost(newPost, user, genPostID, _context);

                        if (createPost != "Success")
                        {
                            BibleVerse.Exceptions.BVException exception = new Exceptions.BVException(createPost, StackTraceRoot, 00001);

                            throw exception;
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
                    else
                    {
                        retryTimes++;
                    }
                }
                return "Failure";
            }catch(Exception ex)
            {
                return "Failure";
            }
            }

        //Get User Profile
        public async Task<ApiResponseModel> GetUserProfile(string userID, string secondUserName)
        {
            IQueryable<Profiles> userProfiles;
            bool profileFound = false;
            int retryTimes = 0;
            ApiResponseModel response = new ApiResponseModel();
            response.ResponseBody = new List<string>();
            response.ResponseErrors = new List<string>();
            UserViewModel suvm = new UserViewModel();
            string userRelationType = "";
            string profileUser = "";

            while(!profileFound && retryTimes < 3)
            {
                if (userID.Count() != 27) // Get User Profile For Account View
                {
                    var username = from c in userManager.Users
                                   where c.UserName == userID
                                   select c;

                  var foundUser = username.First();

                    profileUser = foundUser.UserName;

                    var orgRetrieval = GetUserOrg(foundUser.OrganizationId);
                    BibleVerse.DTO.Organization userOrg = new Organization();

                    if(orgRetrieval.ResponseMessage != "Success")
                    {
                        BibleVerse.Exceptions.BVException x = new Exceptions.BVException(String.Format("Error: \n Root: {0} \n Message: Error during org retrieval", StackTraceRoot));
                        throw x;
                    }else
                    {
                        userOrg = JsonConvert.DeserializeObject<Organization>(GetUserOrg(foundUser.OrganizationId).ResponseBody[0]);
                    }

                    suvm.UserName = foundUser.UserName;
                    suvm.Status = foundUser.Status;
                    suvm.Age = foundUser.Age;
                    suvm.Friends = foundUser.Friends;
                    suvm.Level = foundUser.Level;
                    suvm.OnlineStatus = foundUser.OnlineStatus;
                    suvm.OrgName = userOrg.Name;
                    

                    userProfiles = from c in _context.Profiles
                                   where c.ProfileId == username.First().UserId
                                   select c;

                    var requestResult = from c in _context.UserRelationships
                                        where (c.FirstUser == userID && c.SecondUser == secondUserName) || (c.FirstUser == secondUserName && c.SecondUser == userID)
                                        select c;


                    if(requestResult.FirstOrDefault() != null)
                    {
                        var userRelationship = requestResult.FirstOrDefault();

                        if(userRelationship.FirstUserConfirmed && userRelationship.SecondUserConfirmed) //If both have confirmed relationship, send back relationship
                        {
                            userRelationType = userRelationship.RelationshipType;
                        } else if(userRelationship.FirstUser == secondUserName && userRelationship.SecondUserConfirmed != true) //Else if request user is the relationship sender, provide chance to cancel request
                        {
                            userRelationType = "Cancel " + userRelationship.RelationshipType.ToLower() + " request";
                        } else if(userRelationship.SecondUser == secondUserName && userRelationship.SecondUserConfirmed != true)// Else if request user is the relationship reciever, they can accept request
                        {
                            userRelationType = "Accept " + userRelationship.RelationshipType.ToLower() + " request";
                        }
                    }
                }
                else
                {
                    userProfiles = from c in _context.Profiles
                                   where c.ProfileId == userID
                                   select c;
                }
                if(userProfiles.First() != null)
                {
                    profileFound = true;


                    //Get User Posts
                    var userPosts = from c in _context.Posts
                                    where c.Username == profileUser
                                    select c;

                    response.ResponseMessage = "Success";
                    response.ResponseBody.Add(JsonConvert.SerializeObject(userProfiles.First()));
                    response.ResponseBody.Add(JsonConvert.SerializeObject(suvm));
                    response.ResponseBody.Add(userRelationType);
                    response.ResponseBody.Add(JsonConvert.SerializeObject(userPosts.ToList()));
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

        //Process User Relationship Request
        public async Task<ApiResponseModel> ProcessRelationshipRequest(RelationshipRequestModel request)
        {
            ApiResponseModel response = new ApiResponseModel();
            response.ResponseBody = new List<string>();
            response.ResponseErrors = new List<string>();

            if (request.RequestType == "Send friend request")
            {
                //Verify Relationship doesn't already exist
                var relationList = from c in _context.UserRelationships
                                   where (c.FirstUser == request.FirstUser && c.SecondUser == request.SecondUser && c.RelationshipType == request.RelationshipType) || (c.FirstUser == request.SecondUser && c.SecondUser == request.FirstUser && c.RelationshipType == request.RelationshipType)
                                   select c;

                if (relationList.FirstOrDefault() == null)
                {
                    UserRelationships newRelationship = new UserRelationships()
                    {
                        RelationshipType = request.RelationshipType,
                        FirstUser = request.FirstUser,
                        SecondUser = request.SecondUser,
                        FirstUserConfirmed = true,
                        SecondUserConfirmed = false,
                        ChangeDateTime = DateTime.Now,
                        CreateDateTime = DateTime.Now
                    };

                    bool relationshipRequestSent = BibleVerse.Repositories.UserRespositories.UserRepositoriesHelper.ProcessUserRelationshipSend(newRelationship, request);

                    response.ResponseMessage = relationshipRequestSent ? "Success" : "Failure";

                    if (response.ResponseMessage == "Success")
                    {
                        response.ResponseBody.Add("Friend Request Sent!");
                    }
                }
                else
                {
                    response.ResponseMessage = "Failure";
                    response.ResponseErrors.Add("Relationship Already Exists");
                }
            }
            else if (request.RequestType == "Cancel friend request")
            {
                //Write logic to handle cancel request

                //Find the relationship
                var friendRequest = from x in _context.UserRelationships
                                    where (x.FirstUser == request.FirstUser) && (x.SecondUser == request.SecondUser) && (x.RelationshipType == request.RelationshipType) && x.SecondUserConfirmed == false
                                    select x;

                if (friendRequest.FirstOrDefault() != null) // If relationship is found
                {

                    //Delete notification from db
                    var requestNotifcation = from x in _context.Notifications
                                             where (x.SenderID == request.FirstUser) && (x.RecipientUserID == request.SecondUser) && (x.Type == "Friend Request")
                                             orderby x.CreateDateTime descending
                                             select x;

                    if (requestNotifcation != null)
                    {
                        bool requestWasCancelled = BibleVerse.Repositories.UserRespositories.UserRepositoriesHelper.ProcessUserRelationshipCancel(friendRequest.FirstOrDefault(), request, requestNotifcation.FirstOrDefault());

                        response.ResponseMessage = requestWasCancelled ? "Success" : "Failure";
                    }
                }

            }
            else if (request.RequestType == "Accept friend request")
            {
                //Write logic to handle accept request

                //Find the relationship
                var friendRequest = from x in _context.UserRelationships
                                    where (x.FirstUser == request.SecondUser) && (x.SecondUser == request.FirstUser) && (x.RelationshipType == request.RelationshipType) && x.SecondUserConfirmed == false
                                    select x;

                if (friendRequest.FirstOrDefault() != null) // If relationship is found
                {
                    //Retrieve users
                    var fU = from x in userManager.Users
                             where x.UserName == request.FirstUser
                             select x;

                    var sU = from x in userManager.Users
                             where x.UserName == request.SecondUser
                             select x;

                    if ((fU.FirstOrDefault() != null) && (sU.FirstOrDefault() != null))
                    {
                        Users firstUserFound = fU.FirstOrDefault();
                        Users secondUserFound = sU.FirstOrDefault();

                        bool processRelationship = BibleVerse.Repositories.UserRespositories.UserRepositoriesHelper.ProcessUserRelationshipAccept(friendRequest.FirstOrDefault(), request, firstUserFound, secondUserFound);


                        response.ResponseMessage = processRelationship ? "Success" : "Failure";
                    }
                    else
                    {
                        response.ResponseMessage = "Failure";
                        response.ResponseErrors.Add("Request not found");
                    }
                }
            }
            return response;
        }

        //Delete user post
        public async Task<string> DeleteUserPost(RefreshRequest request, string postId)
        {
            IQueryable<Posts> QPost;
            Users u = _jwtrepository.FindUserFromAccessToken(request); //Find User To Delete Post

            //Find Post
            QPost = from x in _context.Posts
                    where x.PostId == postId
                    select x;

            if(QPost.First() != null)
            {
                Posts p = QPost.First();

                return BibleVerse.Repositories.PostRepositories.PostRepositoriesHelper.DeleteUserPost(p, u);
            } else
            {
                return "Post not found";
            }
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
                    var genPhotoID = BVCommon.BVFunctions.CreateUserID();
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

        //Interact w/ Post Likes
        public async Task<string> InteractWithPostLikes(Likes like, RefreshRequest r)
        {
            Users u = _jwtrepository.FindUserFromAccessToken(r);
            like.LikeUserId = u.UserId;

            if (like.LikeType == "Like")
            {
                //Add Like To Likes Table
                like.CreateDateTime = DateTime.Now;
                _context.Likes.Add(like);
                _context.SaveChanges();

                //Increment Post Likes
                var post = from p in _context.Posts
                           where p.PostId == like.ParentId
                           select p;

                if (post.FirstOrDefault() != null)
                {
                    var p = post.First();
                    p.Likes++;
                    _context.Posts.Update(p);
                    _context.SaveChanges();


                    //Add User History Log
                    UserHistory uhLog = new UserHistory()
                    {
                        UserID = like.LikeUserId,
                        ActionMessage = u.UserName + " just liked a post",
                        ActionType = "Like",
                        Prev_Value = "",
                        Curr_Value = "Like",
                        CreateDateTime = DateTime.Now
                    };

                    _context.UserHistory.Add(uhLog);
                    _context.SaveChanges();

                    //Create Notification For User
                    Notifications notification = new Notifications()
                    {
                        RecipientUserID = p.Username
                    };


                return "Success";
                }
            }
            else
            {
                //Remove Like From Likes Table
                var pl = from l in _context.Likes
                        where (l.LikeUserId == like.LikeUserId) && (l.ParentId == like.ParentId)
                        select l;

                if(pl.FirstOrDefault() != null)
                {
                    var prevLike = pl.First();
                    _context.Likes.Remove(prevLike);

                    var post = from p in _context.Posts
                               where p.PostId == like.ParentId
                               select p;

                    if(post.FirstOrDefault() != null)
                    {
                        post.First().Likes--;
                        _context.Posts.Update(post.First());
                        _context.SaveChanges();
                    }

                    //Add User History Log
                    UserHistory uhLog = new UserHistory()
                    {
                        UserID = like.LikeUserId,
                        ActionMessage = u.UserName + " just unliked a post",
                        ActionType = "Unlike",
                        Prev_Value = "Like",
                        Curr_Value = "",
                        CreateDateTime = DateTime.Now
                    };

                    _context.UserHistory.Add(uhLog);
                    _context.SaveChanges();

                    return "Success";
                }
            }

            return "Failure";
        }

        public async Task<string> InteractWithPostComments(Comments comment, RefreshRequest r) 
        {
            Users u = _jwtrepository.FindUserFromAccessToken(r);
            comment.CommentUserId = u.UserId;
            comment.CreateDateTime = DateTime.Now;

            _context.Comments.Add(comment);
            _context.SaveChanges();

            var pSearch = from x in _context.Posts
                          where x.PostId == comment.ParentId
                          select x;

            if(pSearch != null)
            {
                var post = pSearch.First();
                post.Comments++;
                _context.Posts.Update(post);
                _context.SaveChanges();

                //Add User History Log
                UserHistory uhLog = new UserHistory()
                {
                    UserID = comment.CommentUserId,
                    ActionMessage = u.UserName + " just comment on a post",
                    ActionType = "Comment",
                    Prev_Value = "",
                    Curr_Value = "",
                    CreateDateTime = DateTime.Now
                };

                _context.UserHistory.Add(uhLog);
                _context.SaveChanges();

                return "Success";
            }

            return "Failure";

        }

        public string GetLikeStatus(Users u , string postId)
        {
            Users user = u;

            //See if user liked post
            var likeRow = from x in _context.Likes
                    where (x.ParentId == postId) && (x.LikeUserId == user.UserId)
                    select x;

            if(likeRow.FirstOrDefault() != null)
            {
                return "Unlike";
            }

            return "Like";
        }

        public async Task<ApiResponseModel> Query(string qFilter, string token, string qValue)
        {
            RefreshRequest r = new RefreshRequest()
            {
                AccessToken = token
            };

            //Find User
            Users u = _jwtrepository.FindUserFromAccessToken(r);

            ApiResponseModel response = BibleVerse.Repositories.APIHelperV1.InitializeAPIResponse();
            List<SearchViewModel> searchViews = new List<SearchViewModel>();

            if(qFilter == "ALL")
            {
                IQueryable<Users> userProfiles;
                List<Users> users = new List<Users>();

                userProfiles = from x in _context.Users
                               where x.UserName.Contains(qValue)
                               select x;

                if(userProfiles.FirstOrDefault() != null)
                {
                    foreach(Users user in userProfiles)
                    {
                        SearchViewModel searchView = new SearchViewModel()
                        {
                            UserName = user.UserName,
                            OrgName = "User"
                        };

                        searchViews.Add(searchView);
                    }
                }

                IQueryable<Organization> organizations;
                List<Organization> orgs = new List<Organization>();

                organizations = from x in _context.Organization
                                where x.Name.Contains(qValue)
                                select x;

                if (organizations.FirstOrDefault() != null)
                {
                    foreach (Organization org in organizations)
                    {
                        SearchViewModel searchView = new SearchViewModel()
                        {
                            UserName = org.Name,
                            OrgName = "Group"
                        };

                        searchViews.Add(searchView);
                    }
                }
            }else if(qFilter == "Users")
            {
                IQueryable<Users> userProfiles;
                List<Users> users = new List<Users>();

                userProfiles = from x in _context.Users
                               where x.UserName.Contains(qValue)
                               select x;

                if (userProfiles.FirstOrDefault() != null)
                {
                    foreach (Users user in userProfiles)
                    {
                        SearchViewModel searchView = new SearchViewModel()
                        {
                            UserName = user.UserName,
                            OrgName = "User"
                        };

                        searchViews.Add(searchView);
                    }
                }

            }
            else
            {
                IQueryable<Organization> organizations;
                List<Organization> orgs = new List<Organization>();

                organizations = from x in _context.Organization
                                where x.Name.Contains(qValue)
                                select x;

                if (organizations.FirstOrDefault() != null)
                {
                    foreach (Organization org in organizations)
                    {
                        SearchViewModel searchView = new SearchViewModel()
                        {
                            UserName = org.Name,
                            OrgName = "Group"
                        };

                        searchViews.Add(searchView);
                    }
                }
            }

            if(searchViews.Count > 0)
            {
                response.ResponseMessage = "Success";
                response.ResponseBody.Add(JsonConvert.SerializeObject(searchViews));
            }else
            {
                response.ResponseMessage = "Success";

                SearchViewModel errsvm = new SearchViewModel()
                {
                    UserName = "Nothing Found.",
                    OrgName = "System"
                };
                searchViews.Add(errsvm);
                response.ResponseBody.Add(JsonConvert.SerializeObject(searchViews));
            }

            return response;
        }
    }
}

