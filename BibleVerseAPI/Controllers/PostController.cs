using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BibleVerse.DTO.Repository;
using BibleVerse.DTO;
using Newtonsoft.Json;


namespace BibleVerseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PostController : Controller
    {
        private readonly UserActionRepository _repository;

        public PostController(UserActionRepository repository) => _repository = repository;

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
                    return Ok("No Posts Found");
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

        //Get Timeline Posts
        [HttpGet]
        [ActionName("Profile")]
        public IActionResult Profile(string userID, string currUserName)
        {
            ApiResponseModel userProfile = _repository.GetUserProfile(userID, currUserName).Result;

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
                if (response.Result.ResponseMessage == "Success")
                {
                    return Ok(responseResult);
                }
                else if (response.Result.ResponseMessage == "Failed")
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
            
            var postResponse = _repository.CreateUserPost(post);
            var pr = JsonConvert.SerializeObject(postResponse.ToString());

            if (postResponse != null)
            {
                if (postResponse.Result.ToString() == "Success")
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
        [ActionName("UploadProfilePic")]
        public IActionResult UploadProfilePic([FromBody] object userUpload)
        {
            UserUpload userUpload1 = new UserUpload();
            userUpload1 = JsonConvert.DeserializeObject<UserUpload>(userUpload.ToString());
            
            var uploadResponse = _repository.ChangeUserProfilePic(userUpload1);
            var ur = JsonConvert.SerializeObject(uploadResponse.ToString());

            if (uploadResponse != null)
            {
                if (uploadResponse.Result.ToString() == "Success")
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
