using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Models;
using ServiceRepository.ServiceRepo;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace ServiceRepository
{
    public class ImageRepository : IImageRepository
    {
        private const string Container = "blobs";

        public Task<Response<bool>> RemoveImageFromAzure(string url)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<string>> UploadImageToAzure(HttpContent request)
        {
            try
            {
                string[] _supportedMimeTypes = { "image/png", "image/jpeg", "image/jpg" };
                var httprequest = HttpContext.Current.Request;
                ////var model = httprequest.Form["model"];
                if (!request.IsMimeMultipartContent("form-data"))
                {
                    return new Response<string>() { StatusCode = System.Net.HttpStatusCode.UnsupportedMediaType };
                }

                var accountName = ConfigurationManager.AppSettings["storage:account:name"];
                var accountKey = ConfigurationManager.AppSettings["storage:account:key"];
                var storagePath = ConfigurationManager.AppSettings["storagePath"];
                var storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer imagesContainer = blobClient.GetContainerReference(Container);
                var provider = new AzureStorageMultipartFormDataStreamProvider(imagesContainer);

                try
                {
                    await request.ReadAsMultipartAsync(provider);
                }
                catch (Exception ex)
                {
                    return new Response<string>() { StatusCode = System.Net.HttpStatusCode.BadRequest, Message = $"An error has occured.Details: { ex.Message}" };
                    
                }

                // Retrieve the filename of the file you have uploaded
                var filename = provider.FileData.FirstOrDefault()?.LocalFileName;
                var filePath = string.Concat(storagePath, filename);
                if (string.IsNullOrEmpty(filename))
                {
                    return new Response<string>() { StatusCode = System.Net.HttpStatusCode.BadRequest, Message = "An error has occured while uploading your file. Please try again." };
                }
                return new Response<string>() { StatusCode = HttpStatusCode.OK, Message = $"{filePath}" };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
