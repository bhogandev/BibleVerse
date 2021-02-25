﻿using System;
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
    public class UserController : ControllerBase
    {
        private readonly BibleVerse.Repositories.UserRepositories.UserActionRepository _repository;

        public UserController(BibleVerse.Repositories.UserRepositories.UserActionRepository repository)
        {
            this._repository = repository;
        }

        [HttpGet]
        [ActionName("Query")]
        public IActionResult Index()
        {
            //Get JWT from Request Header
            var token = Request.Headers["Token"];
            var qFilter = Request.Headers["qFilter"];
            var query = Request.Headers["Query"];


            //Pass valid token here
            var response = _repository.Query(qFilter, token, query);

            
            if (response.Result.ResponseMessage == "Success")
            {
                return Ok(response);

            }
            else if (response.Result.ResponseMessage == "Failure")
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
            //Get JWT from Request Header
            var token = Request.Headers["Token"];
            var refreshToken = Request.Headers["RefreshToken"];
            var profileUserName = Request.Headers["UserName"];


           Task<ApiResponseModel> response = _repository.GetUserProfile(token, refreshToken, profileUserName);

            if(response.IsCompleted && response.Result != null)
            {
                if(response.Result.ResponseMessage == "Success")
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
        }
    }
}