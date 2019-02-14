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

        public RegistrationController(IUserRepository userRepository, ILoggers loggers)
        {
            this.userRepository = userRepository;
            this.loggers = loggers;
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
        public async Task<HttpResponseMessage> Post([FromBody]UserDetails user)
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
                
                var res = await userRepository.RegisterUser(userLoginDetails, userdetails);
                if (res?.StatusCode != HttpStatusCode.OK)
                {
                    return new HttpResponseMessage()
                    {
                        StatusCode = res.StatusCode,
                        Content = new StringContent(res.Message)
                    };
                }
                else
                {
                    return new HttpResponseMessage() { StatusCode = res.StatusCode};
                }

            }
            catch (Exception e)
            {
                loggers.LogError(e);
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError};
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
