using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServiceRepository
{
    public interface IImageRepository
    {
        Task<Response<string>> UploadImageToAzure(HttpContent request);

        Task<Response<bool>> RemoveImageFromAzure(string  url);
    }
}
