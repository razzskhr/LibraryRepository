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
            try
            {
                List<BookDetails> bookList = new List<BookDetails>();
                var database = LibManagementConnection.GetConnection();
                var todoTaskCollection = database.GetCollection<BookDetails>(CollectionConstant.Book_Collection);
                var docs = await todoTaskCollection.FindAsync(new BsonDocument());
                await docs.ForEachAsync(doc => bookList.Add(doc));

                return bookList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> AddSubCategoryToExistingBook(ISBNNumber isbnDetails)
        {
            try
            {
                var database = LibManagementConnection.GetConnection();
                var todoTaskCollection = database.GetCollection<BookDetails>(CollectionConstant.Book_Collection);

                if (isbnDetails.id != null)
                {                    
                    isbnDetails.Created = System.DateTime.Now;
                    ObjectId objectId = ObjectId.Parse(isbnDetails.id);
                    var builders = Builders<BookDetails>.Filter.And(Builders<BookDetails>.Filter.Where(x => x.Id == objectId));
                    var update = Builders<BookDetails>.Update.Push("isbnNumber", isbnDetails).Inc("numberOfCopies", 1);
                    var result = await todoTaskCollection.FindOneAndUpdateAsync(builders, update);
                    if (result != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateBookDetails(BookDetails bookDetails)
        {
            try
            {
                var database = LibManagementConnection.GetConnection();
                var todoTaskCollection = database.GetCollection<BookDetails>(CollectionConstant.Book_Collection);
                ObjectId objectId = ObjectId.Parse(bookDetails?.id);
                var builders = Builders<BookDetails>.Filter.And(Builders<BookDetails>.Filter.Where(x => x.Id == objectId));
                var update = Builders<BookDetails>.Update.Set("name", bookDetails?.Name).Set("author",bookDetails?.Author).Set("publishingYear",bookDetails?.PublishingYear).Set("image",bookDetails?.Image).Set("lastUpdated", System.DateTime.UtcNow);                

                var result = await todoTaskCollection.FindOneAndUpdateAsync(builders, update);
                if (result != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> AddNewBook(BookDetails bookDetails)
        {
            try
            {
                var database = LibManagementConnection.GetConnection();
                var todoTaskCollection = database.GetCollection<BookDetails>(CollectionConstant.Book_Collection);
                              
                bookDetails.Created = System.DateTime.Now;
                bookDetails.ISBNNumber.FirstOrDefault().Created = System.DateTime.Now;
                bookDetails.LastUpdated = System.DateTime.Now;
                bookDetails.NumberOfCopies = 1;
                await todoTaskCollection.InsertOneAsync(bookDetails);
                var newBookId = bookDetails.Id.ToString();
                return newBookId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteBookDetails(int bookISBN)
        {
            try
            {
                var database = LibManagementConnection.GetConnection();
                var todoTaskCollection = database.GetCollection<BookDetails>(CollectionConstant.Book_Collection);
                await todoTaskCollection.DeleteOneAsync(new BsonDocument("isbnNumber", bookISBN));
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
