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
using Microsoft.ApplicationInsights;
using Newtonsoft.Json;
using System.Text;
using LibraryManagement.Helpers;
using Loggers;

namespace LibraryManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BooksController : ApiController
    {
        private IBooksRepository booksRepository;
        private ILoggers loggers;
        public BooksController(IBooksRepository booksRepository, ILoggers loggers)
        {
            this.booksRepository = booksRepository;
            this.loggers = loggers;
        }
        [Authorize(Roles = "Student,Admin")]
        // GET: api/Books
        public async Task<List<BookDetails>> Get()
        {
            List<BookDetails> bookDetails = null;
            try
            {
                bookDetails = await booksRepository.GetAllBooks();
            }
            catch (Exception ex)
            {
                loggers.LogError(ex);
            }

            return bookDetails;
        }

        // GET: api/Books/5
        public string Get(int id)
        {
            return "values123";
        }

        [HttpPost]
        [Route("api/Books/AddNewCategoryBook")]
        // POST: api/Books
        public async Task<IHttpActionResult> AddNewCategoryBook([FromBody]BookDetails bookDetails)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await booksRepository.AddNewBook(bookDetails);
                    if (result != null)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                    return BadRequest(ModelState);

            }
            catch (Exception ex)
            {
                loggers.LogError(ex);
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("api/Books/AddISBNDetails")]
        // POST: api/Books
        public async Task<HttpResponseMessage> AddISBNDetails([FromBody]ISBNNumber isbnDetails)
        {
            try
            {
                var result = await booksRepository.AddSubCategoryToExistingBook(isbnDetails);
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    return new HttpResponseMessage() { StatusCode = result.StatusCode, Content = new JsonContent(result.Message) };
                }
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                loggers.LogError(ex);
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new StringContent(JsonConvert.SerializeObject(ex.Message)) };
            }
        }

        // PUT: api/Books/5
        public async Task<IHttpActionResult> Put([FromBody]BookDetails bookDetails)
        {
            try
            {
                var result = await booksRepository.UpdateBookDetails(bookDetails);

                return Ok();
            }
            catch (Exception ex)
            {
                loggers.LogError(ex);
                return NotFound();
            }
        }

        //DELETE: api/Books/5
        public async Task<HttpResponseMessage> Delete([FromBody]ISBNNumber isbnDetails)
        {
            try
            {
                var result = await booksRepository.DeleteBookDetails(isbnDetails);
                if (result)
                {
                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
                }
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.BadRequest };
            }
            catch (Exception ex)
            {
                loggers.LogError(ex);
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = new StringContent(JsonConvert.SerializeObject(ex.Message)) };
            }

        }
    }
}
