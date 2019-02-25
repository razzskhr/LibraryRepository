using Common.EncryptionRepository;
using Loggers;
using Models;
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
        private IUserRepository userRepository;
        private ILoggers loggers;
        private IPasswordRepository passwordRepository;

        public RegistrationController(IUserRepository userRepository, ILoggers loggers,IPasswordRepository passwordRepository)
        {
            this.userRepository = userRepository;
            this.loggers = loggers;
            this.passwordRepository = passwordRepository;
        }
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
        [HttpPost]
        [Route("api/Registration/AddNewUser")]
        public async Task<IHttpActionResult> AddNewUser([FromBody]UserDetails user)
        {
            try
            {
                if (ModelState.IsValid)
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
                    userdetails.RoleType = RoleType.Student;

                    var res = await userRepository.RegisterUser(userLoginDetails, userdetails);
                    if (res?.StatusCode != HttpStatusCode.OK)
                    {
                        return BadRequest(res.Message);
                    }
                    else
                    {
                        return Ok();
                    }
                }
                else
                    return BadRequest(ModelState);

            }
            catch (Exception e)
            {
                loggers.LogError(e);
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

        //[Authorize]
        [HttpPost]
        [Route("api/Registration/ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword([FromBody]LoginDetails loginUserDetails)
        {
            var response = await userRepository.UpdatePassword(loginUserDetails);
            if(response)
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpGet]
        [Route("api/Registration/CheckUser")]
        public async Task<bool> FetchUserName(string userName)
        {
            try
            {
                bool message = await userRepository.CheckUserNameAvailability(userName);
                return message;
            }
            catch(Exception e)
            {
                loggers.LogError(e);
                return false;
            }
          
        }
    }
}
