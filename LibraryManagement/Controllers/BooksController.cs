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
using System.Web;
using Models.Model;

namespace LibraryManagement.Controllers
{

    public class BooksController : ApiController
    {
        private IBooksRepository booksRepository;
        private ILoggers loggers;
        private IUserRepository userRepository;
        private IImageRepository imageRepository;
        public BooksController(IBooksRepository booksRepository, ILoggers loggers, IUserRepository userRepository, IImageRepository imageRepository)
        {
            this.booksRepository = booksRepository;
            this.loggers = loggers;
            this.userRepository = userRepository;
            this.imageRepository = imageRepository;
        }

        //[Authorize]
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

        //[Authorize]
        [Route("api/Books/GetAllAvailableBooks")]
        // GET: api/Books
        public async Task<IHttpActionResult> GetAllAvailableBooks()
        {
            List<BookDetails> bookDetails = null;
            var re = Request;
            var headers = re.Headers;
            var token = headers.Authorization.Parameter;

            if (headers.Contains("Authorization"))
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync("https://oauth2.googleapis.com/tokeninfo?access_token=" + token);
               
                if(response.StatusCode == HttpStatusCode.OK )
                {
                   
                    try
                    {
                        bookDetails = await booksRepository.GetAllAvailableBooks();
                       
                    }
                    catch (Exception ex)
                    {
                        loggers.LogError(ex);
                        return InternalServerError();
                    }
                }
            }
            return Ok(bookDetails);


        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("api/Books/AddNewCategoryBook")]
        // POST: api/Books
        public async Task<IHttpActionResult> AddNewCategoryBook()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var httprequest = HttpContext.Current.Request;
                    var model = httprequest.Form["model"];
                    var bookDetails = JsonConvert.DeserializeObject<BookDetails>(model);
                    var res = await imageRepository.UploadImageToAzure(Request.Content);
                    ////var result = await booksRepository.AddNewBook(bookDetails, res.Message);
                    if (res.StatusCode != HttpStatusCode.OK)
                    {
                        return BadRequest(res.Message);
                    }
                    else
                    {
                        var result = await booksRepository.AddNewBook(bookDetails, res.Message);
                        if (result != null)
                        {
                            return Ok(result);
                        }
                        else
                        {
                            return NotFound();
                        }
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
            try
            {
                var isBookDeleted = await userRepository.UserReturnBooks(issueBooks);
                var isAvailableCopiesIncreased = booksRepository.ReturnBooks(issueBooks);
                if (isBookDeleted && isAvailableCopiesIncreased)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                loggers.LogError(e);
                return InternalServerError();
            }

        }

        [HttpPost]
        [Route("api/Books/IssueBooks")]
        public async Task<IHttpActionResult> IssueBooks([FromBody]IssueBooks issueBooks)
        {
            try
            {
                var isBookAddedToUser = await userRepository.IssueBooksToUser(issueBooks);
                var isAvailableCopiesDecreased = await booksRepository.IssueBooks(issueBooks);
                if (isBookAddedToUser && isAvailableCopiesDecreased)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                loggers.LogError(e);
                return InternalServerError();
            }

        }
        [HttpPost]
        [Route("api/Books/BlockBooks")]
        public async Task<IHttpActionResult> BlockBooks([FromBody] BlockBooks blockedBooks)
        {
            try
            {
                var isBookBlocked = await booksRepository.BlockBooks(blockedBooks);
                if (isBookBlocked)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception e)
            {
                loggers.LogError(e);
                return InternalServerError();
            }
        }
        [HttpPost]
        [Route("api/Books/UnBlockBooks")]
        public async Task<IHttpActionResult> UnBlockBooks([FromBody] BlockBooks blockedBooks)
        {
            try
            {
                var isBookUnBlocked = await booksRepository.UnBlockBooks(blockedBooks);
                if (isBookUnBlocked)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                loggers.LogError(e);
                return InternalServerError();
            }

        }

        [HttpGet]
        [Route("api/Books/GetLatestBookDetails")]
        public async Task<IHttpActionResult> GetLatestBookDetails()
        {
            try
            {
                var res = await booksRepository.GetAllLatestBookDetails();
                if (res != null)
                {
                    return Ok(res);
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception e)
            {
                loggers.LogError(e);
                return InternalServerError();
            }

        }

        //[Authorize]
        [HttpGet]
        [Route("api/Books/GetAllIsbnDetails")]
        public async Task<IHttpActionResult> GetAllIsbnDetails()
        {
            try
            {
                var res = await booksRepository.GetAllIsbnDetails();
                if (res != null)
                {
                    return Ok(res);
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception e)
            {
                loggers.LogError(e);
                return InternalServerError();
            }

        }


        [HttpPost]
        [Route("api/Books/EditBook")]
        public async Task<IHttpActionResult> EditBookDetails([FromBody] ISBNNumber iSBNNumber)
        {
            try
            {
                var res = await booksRepository.EditIsbnDetails(iSBNNumber);
                if (res)
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception e)
            {
                loggers.LogError(e);
                return InternalServerError();
            }

        }

        [HttpGet]
        [Route("api/Books/GetDashbaordDetails")]
        public async Task<IHttpActionResult> GetDashbaordDetails()
        {
            try
            {
                DashboardDetails dashboardDetails = new DashboardDetails();
                var userList = await userRepository.GetAllUsers();
                var bookList = await booksRepository.GetAllBooks();
                if (userList != null)
                {
                    var blockedBooks = userList.Sum(x => x.BlockedCopies);
                    dashboardDetails.RegisteredUsers = userList.Count;
                    dashboardDetails.BlockedBooksCount = blockedBooks;
                }
                if (bookList != null)
                {
                    dashboardDetails.IssuedBooksCount = bookList.Sum(x => x.AvailableCopies);
                }
                dashboardDetails.ConfigrationValuesCount = 3;

                return Ok(dashboardDetails);
            }
            catch (Exception e)
            {
                loggers.LogError(e);
                return InternalServerError();
            }
        }


    }
}
