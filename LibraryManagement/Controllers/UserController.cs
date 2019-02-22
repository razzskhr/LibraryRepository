using Loggers;
using Models;
using Newtonsoft.Json;
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

        [Authorize]
        [HttpPost]
        [Route("api/User/UploadImage")]
        public async Task<HttpResponseMessage> UploadImage()
        {
            try
            {
                UploadImage image = new UploadImage();
                var res = await image.UploadImageToAzure(Request.Content);
                if (res.StatusCode != HttpStatusCode.OK)
                {
                    return new HttpResponseMessage() { StatusCode = res.StatusCode };
                }
                else
                {
                    var identityClaims = (ClaimsIdentity)User.Identity;
                    var res1 = await userRepository.InsertImageFileName(identityClaims.FindFirst("UserName").Value, res.Message);
                    if (res1.StatusCode != HttpStatusCode.OK)
                    {
                        return new HttpResponseMessage() { StatusCode = HttpStatusCode.BadRequest };
                    }
                    else
                    {
                        return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new StringContent(res.Message) };
                    }
                }

            }
            catch (Exception e)
            {
                loggers.LogError(e);
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/GetMailList")]
        public async Task<IEnumerable<object>> GetMailList()
        {
            try
            {
                var result = await userRepository.GetUserMailList();
                return result;
            }
            catch (Exception ex)
            {
                loggers.LogError(ex);
            }
            return new List<object>();
        }

        // POST: api/User
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/User/5
        public void Put(int id, [FromBody]string value)
        {
        }

        [Authorize(Roles ="Admin")]
        // DELETE: api/User/5
        public async Task<HttpResponseMessage> Delete([FromBody]LoginDetails user)
        {
            try
            {
                var result = await userRepository.DeleteUser(user.UserName);
                if (result)
                {
                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
                }
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.BadRequest };
            }
            catch (Exception ex)
            {
                loggers.LogError(ex);
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new StringContent(JsonConvert.SerializeObject(ex.Message)) };
            }

        }
    }
}
