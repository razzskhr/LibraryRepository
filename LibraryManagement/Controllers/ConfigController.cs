using Loggers;
using Models;
using ServiceRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LibraryManagement.Controllers
{
    [Authorize]
    public class ConfigController : ApiController
    {
        private readonly IConfigRepository configRepository;
        private readonly ILoggers logger;
        public ConfigController(IConfigRepository configRepository, ILoggers logger)
        {
            this.configRepository = configRepository;
            this.logger = logger;
        }
        // GET: api/Config
        public async Task<ConfigDetails> Get()
        {
            ConfigDetails configDetails = null;
            try
            {
                configDetails = await configRepository.GetConfigDetails();
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
            }
            return configDetails;
        }

        // GET: api/Config/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Config
        public async Task<bool> Post([FromBody]ConfigDetails configDetails)
        {
            var result = false;
            try
            {
                result = await configRepository.UpdateConfigDetails(configDetails);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
            }
            return result;
        }

        // PUT: api/Config/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Config/5
        public void Delete(int id)
        {
        }
    }
}
