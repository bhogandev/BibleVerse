using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BVCommon;
using BibleVerse.Repositories;
using BibleVerse.DTO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Web;
using BibleVerseDTO.Services;
using System.Text;


namespace BibleVerseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : BVController
    {
        private readonly BibleVerse.Repositories.UserRepositories.UserActionRepository _repository;
        private readonly BibleVerse.Repositories.APIHelperV1 _apiHelper;
        

        public UserController(BibleVerse.Repositories.UserRepositories.UserActionRepository repository, BibleVerse.Repositories.APIHelperV1 apiHelper)
        {
            this._repository = repository;
            this._apiHelper = apiHelper;
        }

        [HttpGet]
        [ActionName("Query")]
        public IActionResult Index()
        {
            //Get JWT from Request Header
            var qFilter = Request.Headers["qFilter"];
            var query = Request.Headers["Query"];


            //Pass valid token here
            var response = _repository.Query(qFilter, Request.Headers["Token"], query);

            
            if (response.Result.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
            {
                return Ok(response);

            }
            else if (response.Result.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Failure))
            {
                return Conflict(response);
            }
            else
            {
                // Create an ELog and Log error
                return BadRequest("An Unexpected Error Occured");
            }
        }

        [HttpGet]
        [ActionName("GetProfile")]
        public IActionResult GetProfile()
        {
            try
            {
                //Get JWT from Request Header
                var profileUserName = Request.Headers["UserName"];

                Task<ApiResponseModel> response = _repository.GetUserProfile(Request.Headers["Token"], Request.Headers["RefreshToken"], profileUserName);

                if (response.IsCompleted && response.Result != null)
                {
                    if (response.Result.ResponseMessage == APIHelperV1.RetreieveResponseMessage(APIHelperV1.ResponseMessageEnum.Success))
                    {
                        return Ok(response.Result);
                    }
                    else
                    {
                        return Conflict(response.Result);
                    }
                }
                else
                {
                    return BadRequest("An Error Occured");
                }
            }catch(ArgumentNullException nullEx)
            {
                //Record Exception
                return BadRequest(BibleVerse.Exceptions.BVExErrorCodes.ExShortCut(BibleVerse.Exceptions.BVExErrorCodes.ShortCodes.KillCode));
            }catch(Exception ex)
            {
                //Log Exception
                return BadRequest();
            }
        }
    }
}
