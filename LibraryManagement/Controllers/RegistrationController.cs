﻿using Models;
using ServiceRepository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LibraryManagement.Controllers
{
    public class RegistrationController : ApiController
    {
        // GET: api/Registration
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Registration/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Registration
        public async Task<IHttpActionResult> Post([FromBody]UserDetails user)
        {
            try
            {
                var userLoginDetails = new LoginDetails()
                {
                    Password = user.Password,
                    UserName = user.UserName,
                    UserID = user.UserID,
                   
                };
                var userdetails = user;
                userdetails.Created = DateTime.Now;
                userdetails.LastUpdated = DateTime.Now;
                userdetails.Password = null;


                UserRepository userRepository = new UserRepository();
                var res = await userRepository.RegisterUser(userLoginDetails, userdetails);
                if(res)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("User already exists");
                }

            }
            catch (Exception e)
            {
                return InternalServerError();
            }

        }

        // PUT: api/Registration/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Registration/5
        public void Delete(int id)
        {
        }
    }
}
