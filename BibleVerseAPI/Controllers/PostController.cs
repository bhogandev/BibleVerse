using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BibleVerse.Repositories;
using BibleVerse.DTO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace BibleVerseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PostController : BVController
    {
        private readonly BibleVerse.Repositories.UserRepositories.UserActionRepository _repository;
        private readonly JWTSettings _jwtSettings;
        private readonly JWTRepository _jWTRepository;
        private readonly APIHelperV1 _apihelper;
        private string serviceBase = "Post";
        private string context = String.Empty;

        public PostController(BibleVerse.Repositories.UserRepositories.UserActionRepository repository, APIHelperV1 apiHelper)
        { 
            _repository = repository;
            _apihelper = apiHelper;
            //Initialize();
        }

        private void Initialize()
        {
            try
            {
                VerifyToken();
                RecordTransaction();
            }
            catch (ArgumentNullException nullEx)
            {

            }
            catch (Exception ex)
            {
                //Log Exception In DB
            }
        }

        //Verify that call provided token and refresh token
        private void VerifyToken()
        {
            if (Request != null)
            {
                string token = string.Empty;
                string refreshToken = string.Empty;

                if ((String.IsNullOrEmpty(Request.Headers["token"]) || Request.Headers["token"] == "undefined") || (String.IsNullOrEmpty(Request.Headers["refreshToken"]) || Request.Headers["RefreshToken"] == "undefined"))
                    throw new ArgumentNullException("Null Value Provided For Token");
            }
        }

        private void RecordTransaction()
        {
            if (Request != null)
            {
                string device = string.Empty;
                string identity = string.Empty;
                string version = string.Empty;
                string requestBody = string.Empty;

                if (!string.IsNullOrEmpty(Request.Headers["device"]))
                    device = Request.Headers["device"];

                if (!string.IsNullOrEmpty(Request.Headers["identity"]))
                    identity = Request.Headers["identity"];

                if (!string.IsNullOrEmpty(Request.Headers["version"]))
                    version = Request.Headers["version"];

                if (Request.Body != null)
                    requestBody = Request.Body.ToString();

                BibleVerse.DTO.Transactions transaction = new BibleVerse.DTO.Transactions()
                {
                    ClientAddress = !string.IsNullOrEmpty(HttpContext.Connection.RemoteIpAddress.ToString()) ? HttpContext.Connection.RemoteIpAddress.ToString() : "",
                    ClientDevice = device,
                    ClientIdentity = identity,
                    ClientVersion = version,
                    TransactionController = Request.Path,
                    TransactionType = Request.Method,
                    TransactionPayload = requestBody,
                    TransactionTime = DateTime.Now,
                };


                _apihelper.RecordTransaction(transaction);
            }
        }

        //Get All Of User's Posts
        [HttpGet]
        [ActionName("Get")]
        public IActionResult Get(string userName)
        {
            List<Posts> userPosts = _repository.GetUserPosts(userName).Result;

            if(userPosts !=  null)
            {
                if(userPosts.Count > 0)
                {
                    return Ok(userPosts);
                } else if(userPosts.Count == 0)
                {
                    return Conflict("No Posts Found");
                }
                else
                {
                    return Conflict("Unable to Retrieve Posts");
                }
            } else
            {
                //create Elog Error
                return BadRequest("An Error Occurred");
            }
        }

        [HttpGet]
        [ActionName("GetPost")]
        public IActionResult GetPost()
        {
            //var token = Request.Headers["Token"];
            var postID = Request.Headers["PostId"];

            var response = _repository.GetPost(postID);

            if(response != null)
            {
                return Ok(response);
            }

            return BadRequest("An Error Occxured");
        }

        //Get Timeline Posts
        //Get All Of User's Posts
        [HttpGet]
        //[Authorize]
        [ActionName("GetTimeline")]
        public IActionResult GetTimeline()
        {
            Initialize();

            var token = Request.Headers["Token"];
            var refreshToken = Request.Headers["RefreshToken"];

            List<Posts> userPosts = _repository.GenerateTimelinePosts(token, refreshToken).Result;

            if (userPosts != null)
            {
                if (userPosts.Count > 0)
                {
                    ApiResponseModel response = new ApiResponseModel();
                    response.ResponseBody = new List<string>();
                    response.ResponseErrors = new List<string>();
                    response.ResponseMessage = APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success);
                    response.ResponseBody.Add(JsonConvert.SerializeObject(userPosts));

                    return Ok(response);
                }
                else if (userPosts.Count == 0)
                {
                    return Conflict("No Posts Found");
                }
                else
                {
                    return Conflict("Unable to Retrieve Posts");
                }
            }
            else
            {
                //create Elog Error
                return BadRequest("An Error Occurred");
            }
        }

        [HttpGet]
        [ActionName("Profile")]
        public IActionResult Profile(string userID, string currUserName)
        {
            var token = Request.Headers["Token"];
            var refreshToken = Request.Headers["RefreshToken"];
            var userName = Request.Headers["UserName"];

            ApiResponseModel userProfile = _repository.GetUserProfile(token, refreshToken, userName).Result;

            if (userProfile != null)
            {
                if (userProfile.ResponseBody.Count > 0)
                {
                    return Ok(userProfile);
                }
                else if (userProfile.ResponseBody.Count == 0)
                {
                    return Conflict("No Profile Found");
                }
                else
                {
                    return Conflict("Unable to Retrieve Profile");
                }
            }
            else
            {
                //create Elog Error
                return BadRequest("An Error Occurred");
            }
        }

        
        [HttpPost]
        [ActionName("RelationshipReq")]
        public IActionResult RelationshipRequest([FromBody] object rRequest)
        {
            RelationshipRequestModel relationshipRequest = JsonConvert.DeserializeObject<RelationshipRequestModel>(rRequest.ToString());

            var response = _repository.ProcessRelationshipRequest(relationshipRequest);
            var responseResult = response.Result;
            

            if (response != null)
            {
                if (response.Result.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
                {
                    return Ok(responseResult);
                }
                else if (response.Result.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure))
                {
                    return Conflict(responseResult);
                }
                else
                {
                    return BadRequest(responseResult);
                }
            }
            else
            {
                //Create an Elog error
                return BadRequest("An Error Occurred");
            }
        }
        

        // POST api/values
        [HttpPost]
        [ActionName("CreatePost")]
        public IActionResult CreatePost([FromBody] object userPost)
        {
            PostModel post = new PostModel();
            post = JsonConvert.DeserializeObject<PostModel>(userPost.ToString());
            post.UserId = Request.Headers["Token"];
            var postResponse = _repository.CreateUserPost(post);
            var pr = JsonConvert.SerializeObject(postResponse.ToString());

            if (postResponse != null)
            {
                if (postResponse.Result.ToString() == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
                {
                    return Ok(pr);
                }
                else if (postResponse.Result.ToString() == "Failed")
                {
                    return Conflict(pr);
                }
                else
                {
                    return BadRequest(pr);
                }
            }
            else
            {
                //Create an Elog error
                return BadRequest("An Error Occurred");
            }

        }

        [HttpPost]
        [ActionName("Interact")]
        public IActionResult PostInteract([FromBody] object request)
        {
            var IntType = Request.Headers["IntType"];
            var token = Request.Headers["Token"];
            if (!String.IsNullOrEmpty(IntType) && !String.IsNullOrEmpty(token))
            {
                Task<string> response = null;
                RefreshRequest r = new RefreshRequest() { AccessToken = token };

                if (IntType == "Like" || IntType == "Unlike")
                {
                    Likes like = JsonConvert.DeserializeObject<Likes>(request.ToString());
                     response = _repository.InteractWithPostLikes(like, r);
                }else if(IntType == "Comment")
                {
                    Comments comment = JsonConvert.DeserializeObject<Comments>(request.ToString());
                    response = _repository.InteractWithPostComments(comment, r);
                }

                if (response != null)
                {
                    if (response.Result.ToString() == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
                    {
                        return Ok();
                    }
                    else if (response.Result.ToString() == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure))
                    {
                        return Conflict();
                    }else {
                        return BadRequest();
                    }
                }
                else
                {
                    //Create an Elog error
                    return BadRequest("An Error Occurred");
                }
            }
            else
            {
                return BadRequest("Please Check Headers For Null Or Empty Value");
            }
        }

        [HttpPost]
        [ActionName("DeletePost")]
        public IActionResult DeletePost([FromBody] object deleteRequest)
        {
            var token = Request.Headers["Token"];
            var postID = JsonConvert.DeserializeObject<Posts>(deleteRequest.ToString()).PostId;

            if(!String.IsNullOrEmpty(token) && !String.IsNullOrEmpty(postID))
            {
                RefreshRequest r = new RefreshRequest()
                {
                    AccessToken = token
                };

                var deleteResponse = _repository.DeleteUserPost( r , postID);

                if (deleteResponse != null)
                {
                    if (deleteResponse.Result.ToString() == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
                    {
                        //return success
                        return Ok();

                    }
                    else
                    {
                        //return error msg
                        return Conflict(deleteResponse.Result.ToString());
                    }
                } else
                {
                    return BadRequest(deleteResponse.Result.ToString());
                }
            } else
            {
                return BadRequest("An Error Occured");
            }
        }

        [HttpPost]
        [ActionName("UploadProfilePic")]
        public IActionResult UploadProfilePic([FromBody] object userUpload)
        {
            UserUpload userUpload1 = new UserUpload();
            userUpload1 = JsonConvert.DeserializeObject<UserUpload>(userUpload.ToString());
            
            var uploadResponse = _repository.ChangeUserProfilePic(userUpload1);
            var ur = JsonConvert.SerializeObject(uploadResponse.ToString());

            if (uploadResponse != null)
            {
                if (uploadResponse.Result.ToString() == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
                {
                    return Ok();
                }
                else if (uploadResponse.Result.ToString() == "Failed")
                {
                    return Conflict(ur);
                }
                else
                {
                    return BadRequest(ur);
                }
            }
            else
            {
                //Create an Elog error
                return BadRequest("An Error Occurred");
            }
        }
    }
}
