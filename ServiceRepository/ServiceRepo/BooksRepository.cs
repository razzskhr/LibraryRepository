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

        public async Task<bool> AddNewBook(BookDetails bookDetails)
        {
            try
            {
                var database = LibManagementConnection.GetConnection();
                var todoTaskCollection = database.GetCollection<BookDetails>(CollectionConstant.Book_Collection);
                
                if (bookDetails.id!= null)
                {
                    var book = await todoTaskCollection.FindAsync(new BsonDocument("_id",new ObjectId( bookDetails.id)));
                   await todoTaskCollection.UpdateOneAsync(a => a.Id == bookDetails.Id, Builders<BookDetails>.Update.AddToSet(x => x.ISBNNumber, bookDetails.ISBNNumber.FirstOrDefault()));
                }
                await todoTaskCollection.InsertOneAsync(bookDetails);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteBook(int bookISBN)
        {
            try
            {
                var database = LibManagementConnection.GetConnection();
                var todoTaskCollection = database.GetCollection<BookDetails>(CollectionConstant.Book_Collection);
                await todoTaskCollection.DeleteOneAsync(new BsonDocument("ISBNNumber", bookISBN));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
