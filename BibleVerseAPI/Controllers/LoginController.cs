﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using BibleVerse.DTO.Repository;
using BibleVerse.DTO;
using Newtonsoft.Json;

namespace BibleVerseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly RegistrationRepository _repository;
        private readonly ELogRepository _elogRepository;

        public LoginController(RegistrationRepository repository) => _repository = repository;

       [HttpPost]
       public IActionResult LoginUser([FromBody] object userRequest)
        {
            string lr = "";

            var loginResponse = _repository.LoginUser(JsonConvert.DeserializeObject<LoginRequestModel>(userRequest.ToString()));
            try
            {
                lr = JsonConvert.SerializeObject(loginResponse.Result);
            }catch(Exception ex)
            {
                lr = JsonConvert.SerializeObject(loginResponse.Result);

                //Create ELog Storing Exception
                var error = _elogRepository.LogError("UserLogin", 3, ex.ToString());
            }

            if (loginResponse.IsCompletedSuccessfully)
            {
                if (loginResponse.Result.ResponseStatus == "Success")
                {
                    return Ok(lr);
                }
                else if (loginResponse.Result.ResponseStatus == "Failed")
                {
                    return Conflict(lr);
                }
                else if(loginResponse.Result.ResponseStatus == "Email not confirmed")
                {
                    return Conflict(lr);
                }
                else
                {
                    return BadRequest(lr);
                }
            } else
            {
                //Create an Elog error
                return BadRequest("An Error Occurred");
            }
        }
    }
}
