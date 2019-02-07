using Models;
using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceRepository
{
    public class BooksRepository : IBooksRepository
    {
        public async Task<List<BookDetails>> GetAllBooks()
        {
            List<BookDetails> bookList = new List<BookDetails>();
            var database = LibManagementConnection.GetConnection();
            var todoTaskCollection = database.GetCollection<BookDetails>(CollectionConstant.Book_Collection);
            var docs = await todoTaskCollection.FindAsync(new BsonDocument());
            await docs.ForEachAsync(doc => bookList.Add(doc));

            return bookList;
        }
    }
}
