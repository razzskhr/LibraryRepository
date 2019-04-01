using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;

namespace LibraryManagement.Helpers
{
    public class CustomAuthorizeAttribute: System.Web.Http.AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            HttpResponseMessage response;
            var headers = actionContext.Request.Headers;
            var token = headers.Authorization.Parameter;
            if (headers.Contains("Authorization"))
            {
                HttpClient client = new HttpClient();
                response= client.GetAsync("https://oauth2.googleapis.com/tokeninfo?access_token=" + token).Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                    return false;
            }
            else
            {
                return false;
            }
          
        }
    }
}