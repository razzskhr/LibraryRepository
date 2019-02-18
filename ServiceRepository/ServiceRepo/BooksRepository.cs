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

        public async Task<List<BookDetails>> GetAllAvailableBooks()
        {
            try
            {
                var database = LibManagementConnection.GetConnection();
                var todoTaskCollection = database.GetCollection<BookDetails>(CollectionConstant.Book_Collection);
                var docs = await todoTaskCollection.FindAsync(x => x.NumberOfCopies > 0);
                var bookList = await docs.ToListAsync();
                return bookList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Response<string>> AddSubCategoryToExistingBook(ISBNNumber isbnDetails)
        {
            //IClientSessionHandle session = null;
            try
            {
                var database = LibManagementConnection.GetConnection();
                //session = database.Client.StartSession();
                //session.StartTransaction();
                var todoTaskCollection = database.GetCollection<BookDetails>(CollectionConstant.Book_Collection);

                ObjectId objectId = ObjectId.Parse(isbnDetails.BookID);
                var builders = Builders<BookDetails>.Filter.And(Builders<BookDetails>.Filter.Where(x => x.Id == objectId));
                var bookDetails = await todoTaskCollection.Find(builders).ToListAsync();
                var IsISBNExists = bookDetails.All(x => x.ISBNNumber.All(y => y.TrackNo == isbnDetails.TrackNo));
               
                if (!IsISBNExists && isbnDetails.BookID != null)
                {                    
                    isbnDetails.Created = System.DateTime.Now;                    
                    var update = Builders<BookDetails>.Update.Push("isbnNumber", isbnDetails).Inc("numberOfCopies", 1);
                    var result = await todoTaskCollection.FindOneAndUpdateAsync(builders, update);
                    //session.CommitTransaction();
                    if (result != null)
                    {
                        return new Response<string>() {StatusCode = System.Net.HttpStatusCode.OK };
                    }
                    else
                    {
                        return new Response<string>() { StatusCode = System.Net.HttpStatusCode.NotFound };
                    }
                }
                else
                {
                    return new Response<string>() { StatusCode = System.Net.HttpStatusCode.BadRequest, Message = "ISBN Number Already Exists" };
                }
            }
            catch(Exception ex)
            {
                //session?.AbortTransaction();
                throw ex;
            }
        }

        public async Task<bool> UpdateBookDetails(BookDetails bookDetails)
        {
            //IClientSessionHandle session = null;
            try
            {
                if (bookDetails.BookID != null)
                {
                    var database = LibManagementConnection.GetConnection();
                    //session = database.Client.StartSession();
                    //session.StartTransaction();
                    var todoTaskCollection = database.GetCollection<BookDetails>(CollectionConstant.Book_Collection);
                    ObjectId objectId = ObjectId.Parse(bookDetails.BookID);
                    var builders = Builders<BookDetails>.Filter.And(Builders<BookDetails>.Filter.Where(x => x.Id == objectId));
                    var update = Builders<BookDetails>.Update.Set("name", bookDetails?.Name).Set("author", bookDetails?.Author).Set("publishingYear", bookDetails?.PublishingYear).Set("image", bookDetails?.Image).Set("lastUpdated", System.DateTime.UtcNow);

                    var result = await todoTaskCollection.FindOneAndUpdateAsync(builders, update);
                    //session.CommitTransaction();
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
            catch (Exception ex)
            {
                //session?.AbortTransaction();
                throw ex;
            }
        }

        ////public async Task<bool> UpdateISBNDetails(ISBNNumber isbnDetails)
        ////{
        ////    try
        ////    {
        ////        var database = LibManagementConnection.GetConnection();
        ////        var todoTaskCollection = database.GetCollection<BookDetails>(CollectionConstant.Book_Collection);
        ////        ObjectId objectId = ObjectId.Parse(isbnDetails.id);
        ////        var builders = Builders<BookDetails>.Filter.And(Builders<BookDetails>.Filter.Where(x => x.Id == objectId));
        ////        var update = Builders<BookDetails>.Update.Set("name", bookDetails?.Name).Set("author", bookDetails?.Author).Set("publishingYear", bookDetails?.PublishingYear).Set("image", bookDetails?.Image).Set("lastUpdated", System.DateTime.UtcNow);

        ////        var result = await todoTaskCollection.FindOneAndUpdateAsync(builders, update);
        ////        if (result != null)
        ////        {
        ////            return true;
        ////        }
        ////        else
        ////        {
        ////            return false;
        ////        }
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        throw ex;
        ////    }
        ////}
        
        public async Task<BookDetails> AddNewBook(BookDetails bookDetails)
        {
            //IClientSessionHandle session = null;
            try
            {
                var database = LibManagementConnection.GetConnection();
                //session = database.Client.StartSession();
                //session.StartTransaction();
                var todoTaskCollection = database.GetCollection<BookDetails>(CollectionConstant.Book_Collection);
                              
                bookDetails.Created = System.DateTime.Now;
                bookDetails.ISBNNumber.FirstOrDefault().Created = System.DateTime.Now;
                bookDetails.LastUpdated = System.DateTime.Now;
                bookDetails.NumberOfCopies = 1;
                await todoTaskCollection.InsertOneAsync(bookDetails);
                //session.CommitTransaction();
                return bookDetails;
            }
            catch (Exception ex)
            {
                //session.AbortTransaction();
                throw ex;
            }
        }

        public async Task<bool> DeleteBookDetails(ISBNNumber isbnDetails)
        {
            //IClientSessionHandle session = null;
            try
            {
                var database = LibManagementConnection.GetConnection();
               // session = database.Client.StartSession();
                //session.StartTransaction();
                var todoTaskCollection = database.GetCollection<BookDetails>(CollectionConstant.Book_Collection);
                ObjectId objectId = ObjectId.Parse(isbnDetails.BookID);

                var builders = Builders<BookDetails>.Filter.And(Builders<BookDetails>.Filter.Eq(x => x.Id, objectId));
                var update = Builders<BookDetails>.Update.PullFilter("isbnNumber", Builders<BsonDocument>.Filter.Eq("trackNo", isbnDetails.TrackNo)).Inc("numberOfCopies", -1);

                var result = await todoTaskCollection.FindOneAndUpdateAsync(builders, update);
                //session.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                //session?.AbortTransaction();
                throw ex;
            }
        }

    }
}
