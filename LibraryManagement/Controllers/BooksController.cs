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
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Books/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Books/5
        public void Delete(int id)
        {
        }
    }
}
