﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BibleVerse.DTO;
using BibleVerse.DTO.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BibleVerseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AWSController : Controller
    {
        private readonly AWSRepository _repository;
        private readonly ELogRepository _elogRepository;
        private string serviceBase = "AWS";
        private string context = String.Empty;

        public AWSController(AWSRepository repository, ELogRepository eLogRepository)
        {
            _repository = repository;
            _elogRepository = eLogRepository;
        }

        
        [HttpPost]
        [ActionName("CreateUserDir")]
        public async Task<ObjectResult> CreateUserDir([FromBody] object newUser)
        {
            context = serviceBase + "CreateUserDir";

            var apiResponse = new ApiResponseModel();

            //Run Method in aws repository to create user dir in org bucket
            try
            {
                Users nu = JsonConvert.DeserializeObject<Users>(newUser.ToString());

                apiResponse = await _repository.CreateUserDir(nu);

                if (apiResponse.ResponseMessage == "Success")
                {
                    return Ok(apiResponse);
                }
                else if (apiResponse.ResponseMessage == "Failure")
                {
                    return Conflict(apiResponse);
                }
                else
                {
                    apiResponse.ResponseMessage = "BadRequest";
                    apiResponse.ResponseErrors = new List<string>();
                    apiResponse.ResponseErrors.Add("An Unexpected Error Occured. Please try again");
                    return BadRequest(apiResponse);
                }
            }catch(Exception ex)
            {
                //store err in elog
                ELog eLog = new ELog()
                {
                    CreateDateTime = DateTime.Now,
                    Service = context,
                    Severity = 3,
                    Message = ELog.GenerateElogFromException(ex)
                };

                var eLogStore = _elogRepository.StoreELog(eLog);

                if(!(eLogStore.Result == "Success"))
                {
                    //Store in Event Viewer On Server
                }

                //Send back response
                apiResponse.ResponseMessage = "Error";
                apiResponse.ResponseErrors = new List<string>();
                apiResponse.ResponseErrors.Add("An Unexpected Error Occured. Please try again");
                return Conflict(apiResponse);
            }
        }
        
        
        [HttpPost]
        [ActionName("CreateOrgBucket")]
        public async Task<ObjectResult> CreateOrgBucket([FromBody] object org)
        {
            context = serviceBase + "CreateOrgBucket";

            var apiResponse = new ApiResponseModel();

            try
            {
                Organization newOrg = JsonConvert.DeserializeObject<Organization>(org.ToString());

                 apiResponse = await _repository.CreateOrgBucket(newOrg);

                if (apiResponse.ResponseMessage == "Success")
                {
                    return Ok(apiResponse);
                }
                else if (apiResponse.ResponseMessage == "Failure")
                {
                    return Conflict(apiResponse);
                }
                else
                {
                    apiResponse.ResponseMessage = "BadRequest";
                    apiResponse.ResponseErrors = new List<string>();
                    apiResponse.ResponseErrors.Add("An Unexpected Error Occured. Please try again");
                    return BadRequest(apiResponse);
                }
            }catch(Exception ex)
            {
                //store err in elog
                ELog eLog = new ELog()
                {
                    CreateDateTime = DateTime.Now,
                    Service = context,
                    Severity = 3,
                    Message = ELog.GenerateElogFromException(ex)
                };

                var eLogStore = _elogRepository.StoreELog(eLog);

                if (!(eLogStore.Result == "Success"))
                {
                    //Store in Event Viewer On Server
                }

                //Send back response
                apiResponse.ResponseMessage = "Error";
                apiResponse.ResponseErrors = new List<string>();
                apiResponse.ResponseErrors.Add("An Unexpected Error Occured. Please try again");
                return Conflict(apiResponse);
            }
        }
    }  
}
