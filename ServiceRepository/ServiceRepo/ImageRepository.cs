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

        private CloudBlobContainer blobContainer;

        public void DeleteFile(string uniqueFileIdentifier)
        {

        }




        public async Task<Response<bool>> RemoveImageFromAzure(string url)
        {
            try
            {
                var fileName = url;
                GetStorageReference();
                var blob = this.blobContainer.GetBlockBlobReference(fileName);
                var result = await blob.DeleteIfExistsAsync();
                if (result)
                    return new Response<bool>() { StatusCode = HttpStatusCode.OK, ResultType = ResultType.Success };
                return new Response<bool>() { StatusCode = HttpStatusCode.BadRequest, ResultType = ResultType.Error };

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<Response<string>> UploadImageToAzure(HttpContent request)
        {
            try
            {
                string[] _supportedMimeTypes = { "image/png", "image/jpeg", "image/jpg" };
                var httprequest = HttpContext.Current.Request;
                if (!request.IsMimeMultipartContent("form-data"))
                {
                    return new Response<string>() { StatusCode = System.Net.HttpStatusCode.UnsupportedMediaType };
                }
                GetStorageReference();

                var provider = new AzureStorageMultipartFormDataStreamProvider(blobContainer);

                try
                {
                    await request.ReadAsMultipartAsync(provider);
                }
                catch (Exception ex)
                {
                    return new Response<string>() { StatusCode = System.Net.HttpStatusCode.BadRequest, Message = $"An error has occured.Details: { ex.Message}" };

                }

                var storagePath = ConfigurationManager.AppSettings["storagePath"];
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

        private void GetStorageReference()
        {
            var accountName = ConfigurationManager.AppSettings["storage:account:name"];
            var accountKey = ConfigurationManager.AppSettings["storage:account:key"];
            var storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            blobContainer = blobClient.GetContainerReference(Container);

        }

    }
}
