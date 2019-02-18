using Loggers;
using Models;
using ServiceRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace LibraryManagement.Controllers
{
    public class UserController : ApiController
    {
        private IUserRepository userRepository;
        private ILoggers loggers;

        public UserController(IUserRepository userRepository, ILoggers loggers)
        {
            this.userRepository = userRepository;
            this.loggers = loggers;
        }

        [Authorize(Roles ="Admin")]
        // GET: api/User
        public async Task<List<UserDetails>> Get()
        {
            List<UserDetails> userDetails = null;
            try
            {
                userDetails = await userRepository.GetAllUsers();
            }
            catch (Exception ex)
            {
                loggers.LogError(ex);
            }

            return userDetails;
        }
        
        [HttpGet]
        [Route("api/GetUserClaims")]
        public IHttpActionResult GetUserClaims()
        {
            try
            {
                var identityClaims = (ClaimsIdentity)User.Identity;
                IEnumerable<Claim> claims = identityClaims.Claims;
                var res = userRepository.GetLoggedInUserDetails(identityClaims.FindFirst("UserName").Value);
                UserDetails user = new UserDetails()
                {
                    Email = identityClaims.FindFirst("Email").Value,
                    FirstName = identityClaims.FindFirst("FirstName").Value,
                    LastName = identityClaims.FindFirst("LastName").Value,
                    UserName = identityClaims.FindFirst("UserName").Value,
                    UserID = identityClaims.FindFirst("UserId").Value
                };
                user.RoleType = res.RoleType;
                return Ok(user);
            }
            catch (Exception e)
            {
                loggers.LogError(e);
                return InternalServerError();
            }
        }

        // GET: api/User/5
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet]
        [Route("api/GetMailList")]
        public async Task<List<string>> GetMailList()
        {
            var result = new List<string>();
            try
            {
                 result = await userRepository.GetUserMailList();
            }
            catch (Exception ex)
            {
                loggers.LogError(ex);
            }
            return result;
        }

        // POST: api/User
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/User/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/User/5
        public void Delete(int id)
        {
        }
    }
}
