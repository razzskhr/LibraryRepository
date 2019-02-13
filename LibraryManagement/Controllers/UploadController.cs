using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using ServiceRepository.ServiceRepo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace LibraryManagement.Controllers
{
    public class UploadController : ApiController
    {
        private const string Container = "blobs";

        [HttpGet]
        public string Get()
        {
            return "Upload File";
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post()
        {
            try
            {
                var tt = Request.Content.ReadAsStringAsync().Result;
                
               /// var httprequest = HttpContext.Current.Request;
                if (!Request.Content.IsMimeMultipartContent("form-data"))
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                var accountName = ConfigurationManager.AppSettings["storage:account:name"];
                var accountKey = ConfigurationManager.AppSettings["storage:account:key"];
              
                var storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer imagesContainer = blobClient.GetContainerReference(Container);
                var provider = new AzureStorageMultipartFormDataStreamProvider(imagesContainer);

                try
                {
                    await Request.Content.ReadAsMultipartAsync(provider);
                }
                catch (Exception ex)
                {
                    return BadRequest($"An erro r has occured. Details: {ex.Message}");
                }

                // Retrieve the filename of the file you have uploaded
                var filename = provider.FileData.FirstOrDefault()?.LocalFileName;
                if (string.IsNullOrEmpty(filename))
                {
                    return BadRequest("An error has occured while uploading your file. Please try again.");
                }

                return Ok($"File: {filename} has successfully uploaded");
            }
            catch (Exception ex)
            {

                throw ex;
            }

          
        }
    }

}