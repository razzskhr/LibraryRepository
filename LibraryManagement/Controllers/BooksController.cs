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
    public class BooksController : ApiController
    {
        private IBooksRepository booksRepository;
        private ILoggers loggers;
        public BooksController(IBooksRepository booksRepository, ILoggers loggers)
        {
            this.booksRepository = booksRepository;
            this.loggers = loggers;
        }

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
                    BooksRepository booksRepository = new BooksRepository();
                    var result = await booksRepository.AddNewBook(bookDetails);
                    if (result != null & result.Any())
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
        public async Task<IHttpActionResult> AddISBNDetails([FromBody]ISBNNumber isbnDetails)
        {
            try
            {
                BooksRepository booksRepository = new BooksRepository();
                var result = await booksRepository.AddSubCategoryToExistingBook(isbnDetails);

                return Ok();
            }
            catch (Exception ex)
            {
                loggers.LogError(ex);
                return NotFound();
            }
        }

        // PUT: api/Books/5
        public async Task<IHttpActionResult> Put([FromBody]BookDetails bookDetails)
        {
            try
            {
                BooksRepository booksRepository = new BooksRepository();
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
        public async Task<HttpResponseMessage> Delete(int bookISBN)
        {
            try
            {
                BooksRepository booksRepository = new BooksRepository();
                var result = await booksRepository.DeleteBookDetails(bookISBN);
                if (result)
                {
                    return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
                }
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.BadRequest };
            }
            catch (Exception ex)
            {
                loggers.LogError(ex);
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent(JsonConvert.SerializeObject(ex.Message)) };
            }

        }
    }
}
