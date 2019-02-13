using Models.Model;
using ServiceRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LibraryManagement.Controllers
{
    public class EmailController : ApiController
    {
        private SendGridEmailService _sendGridEmailService;
        public EmailController()
        {
            _sendGridEmailService = new SendGridEmailService();
        }       

        [HttpPost]
        public bool Send(EmailContract emailContract)
        {
            try
            {
                var response = _sendGridEmailService.Send(emailContract);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
