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
using Models.Model;

namespace LibraryManagement.Controllers
{
    
    public class BooksController : ApiController
    {
        private IBooksRepository booksRepository;
        private ILoggers loggers;
        private IUserRepository userRepository;
        public BooksController(IBooksRepository booksRepository, ILoggers loggers,IUserRepository userRepository)
        {
            this.booksRepository = booksRepository;
            this.loggers = loggers;
            this.userRepository = userRepository;
        }

        [Authorize]
        [Route("api/Books/GetAllBooks")]
        // GET: api/Books
        public async Task<IHttpActionResult> GetAllBooks()
        {
            List<BookDetails> bookDetails = null;
            try
            {
                bookDetails = await booksRepository.GetAllBooks();
                return Ok(bookDetails);
            }
            catch (Exception ex)
            {
                loggers.LogError(ex);
                return InternalServerError();
            }

        }

        [Authorize]
        [Route("api/Books/GetAllAvailableBooks")]
        // GET: api/Books
        public async Task<IHttpActionResult> GetAllAvailableBooks()
        {
            List<BookDetails> bookDetails = null;
            try
            {
                bookDetails = await booksRepository.GetAllAvailableBooks();
                return Ok(bookDetails);
            }
            catch (Exception ex)
            {
                loggers.LogError(ex);
                return InternalServerError();
            }            
        }
        
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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
        [HttpPost]
        [Route("api/Books/ReturnBooks")]
        public async Task<IHttpActionResult> ReturnBooks([FromBody]IssueBooks issueBooks)
        {
            var isBookDeleted = await userRepository.UserReturnBooks(issueBooks);
           var isAvailableCopiesIncreased =  booksRepository.ReturnBooks(issueBooks);
            if (isBookDeleted && isAvailableCopiesIncreased)
            {
                return Ok();
            }
            else
            {
                return BadRequest(); 
            }
        }

        [HttpPost]
        [Route("api/Books/IssueBooks")]
        public async Task<IHttpActionResult> IssueBooks([FromBody]IssueBooks issueBooks)
        {
           var isBookAddedToUser=await userRepository.IssueBooksToUser(issueBooks);
           //var isAvailableCopiesDecreased= await booksRepository.IssueBooks(issueBooks);
            if (isBookAddedToUser /*&& isAvailableCopiesDecreased*/)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("api/Books/BlockBooks")]
        public async Task<IHttpActionResult> BlockBooks([FromBody] BlockBooks blockedBooks)
        {
            var isBookBlocked=await booksRepository.BlockBooks(blockedBooks);
            return Ok();
        }
        [HttpPost]
        [Route("api/Books/UnBlockBooks")]
        public async Task<IHttpActionResult> UnBlockBooks([FromBody] BlockBooks blockedBooks)
        {
            var isBookBlocked=await booksRepository.UnBlockBooks(blockedBooks);
            return Ok();
        }

        [HttpPost]
        [Route("api/Books/GetAllBooksByUserId")]
        public List<IssueBooks> GetAllBooksByUserId([FromBody]UserDetails userDetails)
        {
            var IssuesBookDetails=userRepository.GetAllIssuedbooksToUser(userDetails.UserID);
            return IssuesBookDetails;
        }

        public async Task<IEnumerable<object>> GetLatestBookDetails()
        {
            var res=await booksRepository.GetAllLatestBookDetails();
            return res;
        }
    }
}
