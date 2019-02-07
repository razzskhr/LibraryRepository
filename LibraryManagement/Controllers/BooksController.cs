using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MongoDB.Driver;
using System.Web.Http;
using Models;
using ServiceRepository;
using Newtonsoft.Json;
using System.Text;

namespace LibraryManagement.Controllers
{
    public class BooksController : ApiController
    {
        // GET: api/Books
        public async Task<List<BookDetails>> Get()
        {
            List<BookDetails> bookDetails = null;
            try
            {
                BooksRepository booksRepository = new BooksRepository();
                bookDetails = await booksRepository.GetAllBooks();
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return bookDetails;
        }

        // GET: api/Books/5
        public string Get(int id)
        {
            return "values123";
        }

        // POST: api/Books
        public async Task<IHttpActionResult> Post([FromBody]BookDetails bookDetails)
        {
            try
           {
                
            BooksRepository booksRepository = new BooksRepository();
            var result = await booksRepository.AddNewBook(bookDetails);
            
            return Ok();
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        // PUT: api/Books/5
            public void Put(int id, [FromBody]string value)
        {
        }

        //DELETE: api/Books/5
        //public async Task<HttpResponseMessage> Delete(int bookISBN)
        //{
        //    try
        //    {
        //        BooksRepository booksRepository = new BooksRepository();
        //        var result = await booksRepository.DeleteBookDetails(bookISBN);
        //        if (result)
        //        {
        //            return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
        //        }
        //        return new HttpResponseMessage() { StatusCode = HttpStatusCode.BadRequest };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new HttpResponseMessage() { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent(JsonConvert.SerializeObject(ex.Message)) };
        //    }

        //}
    }
}
