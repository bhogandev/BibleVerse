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
    [Route("api/[controller]")]
    public class PostController : Controller
    {
        private readonly UserActionRepository _repository;

        public PostController(UserActionRepository repository) => _repository = repository;

        //Get All Of User's Posts
        [HttpGet]
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

        // POST api/values
        [HttpPost]
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
    }
}
