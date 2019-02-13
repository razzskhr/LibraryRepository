using Models;
using Models.Model;
using MongoDB.Driver;
using ServiceRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LibraryManagement.Controllers
{
    public class RolesController : ApiController
    {
        [HttpGet]
        [Route("api/GetAllRoles")]
        // GET: api/Roles
        public HttpResponseMessage GetRoles()
        {
            var database = LibManagementConnection.GetConnection();
            var todoTaskCollection = database.GetCollection<RoleDetails>(CollectionConstant.Roles_Collection);
            var rolesList =  todoTaskCollection.Find(FilterDefinition<RoleDetails>.Empty).ToListAsync();
            return this.Request.CreateResponse(HttpStatusCode.OK, rolesList);
        }

        // GET: api/Roles/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Roles
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Roles/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Roles/5
        public void Delete(int id)
        {
        }
    }
}
