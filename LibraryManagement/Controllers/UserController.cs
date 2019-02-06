using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LibraryManagement.Controllers
{
    public class UserController : ApiController
    {
        // GET: api/User
        public async Task<List<UserDetails>> Get()
        {
            List<UserDetails> userList = new List<UserDetails>();
            try
            {
               
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return userList;
        }

        // GET: api/User/5
        public string Get(int id)
        {
            return "value";
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
