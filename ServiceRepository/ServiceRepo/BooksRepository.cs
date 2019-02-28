using Models;
using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Model;

namespace ServiceRepository
{
    public class BooksRepository : IBooksRepository
    {
        private IConfigRepository configRepository;
        public BooksRepository(IConfigRepository configRepository)
        {
            this.configRepository = configRepository;
        }
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
                    var update = Builders<BookDetails>.Update.Push("isbnNumber", isbnDetails).Inc("numberOfCopies", 1).Inc("availableCopies", 1);
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
                    var update = Builders<BookDetails>.Update.Set("name", bookDetails?.Name).Set("image", bookDetails?.Image).Set("lastUpdated", System.DateTime.UtcNow);

                    //.Set("publishingYear", bookDetails?.PublishingYear)

                    //.Set("author", bookDetails?.Author)

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
        
        public async Task<BookDetails> AddNewBook(BookDetails bookDetails, string image)
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
                bookDetails.NumberOfCopies = bookDetails.NumberOfCopies +1;
                bookDetails.AvailableCopies = bookDetails.AvailableCopies + 1;
                bookDetails.Image = image;
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

       public bool ReturnBooks(IssueBooks issueBooks)
        {
            try
            {
                var database = LibManagementConnection.GetConnection();

                var booksDetailsCollection = database.GetCollection<BookDetails>(CollectionConstant.Book_Collection);
                ObjectId objectId = ObjectId.Parse(issueBooks.BookID);
                var data = booksDetailsCollection.Find(x => x.Id == objectId).First();
                data.AvailableCopies = data.AvailableCopies + 1;
                var details = data.ISBNNumber.First(i => i.TrackNo == issueBooks.ISBNNumber);
                details.Occupied = false;
                var result = booksDetailsCollection.ReplaceOne(c => c.Id == data.Id, data);
                if (result.IsModifiedCountAvailable)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> IssueBooks(IssueBooks issueBooks)
        {
            try
            {
                var database = LibManagementConnection.GetConnection();
                var bookDetailsCollection = database.GetCollection<BookDetails>(CollectionConstant.Book_Collection);
                ObjectId objectId = ObjectId.Parse(issueBooks.BookID);
                var data=await bookDetailsCollection.FindAsync(x => x.Id == objectId);
                var bookdata = await data.ToListAsync();
                var bookDetails=bookdata.FirstOrDefault();
                var blockedbookDetails = bookDetails.BlockBooks.Where(x => x.ISBNNumber == issueBooks.ISBNNumber).ToList();
                if(blockedbookDetails.Count > 0)
                {
                    bookDetails.BlockedCopies = bookDetails.BlockedCopies - 1;
                    bookDetails.ISBNNumber.First(i => i.TrackNo == issueBooks.ISBNNumber).Occupied=true;
                    bookDetails.BlockBooks = new List<BlockBooks>();
                    var result = bookDetailsCollection.ReplaceOne(c => c.Id == bookDetails.Id, bookDetails);
                    //var builders = Builders<BookDetails>.Filter.And(Builders<BookDetails>.Filter.Eq(x => x.Id, objectId));
                    //var update = Builders<BookDetails>.Update.PullFilter("blockedBooks", Builders<BsonDocument>.Filter.Eq("isbnNumber", issueBooks.ISBNNumber)).Inc("blockedCopies", -1).;
                    //var response  = await bookDetailsCollection.FindOneAndUpdateAsync(builders, update);
                    return true;
                }
                else
                {
                    var details = bookDetails.ISBNNumber.First(i => i.TrackNo == issueBooks.ISBNNumber);
                    bookDetails.AvailableCopies = bookDetails.AvailableCopies - 1;
                    details.Occupied = true;
                    var result = bookDetailsCollection.ReplaceOne(c => c.Id == bookDetails.Id, bookDetails);
                    return true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> BlockBooks(BlockBooks blockedbookdetails)
        {
            try
            {
                var database = LibManagementConnection.GetConnection();
                var bookDetailsCollection = database.GetCollection<BookDetails>(CollectionConstant.Book_Collection);
                ObjectId objectId = ObjectId.Parse(blockedbookdetails.BookID);
                var builders = Builders<BookDetails>.Filter.And(Builders<BookDetails>.Filter.Eq(x => x.Id, objectId), Builders<BookDetails>.Filter.ElemMatch(x => x.ISBNNumber, c => c.TrackNo == blockedbookdetails.ISBNNumber));
                var bookDetails = await bookDetailsCollection.Find(builders).ToListAsync();
                var configDetails =await configRepository.GetConfigDetails();
                var data = bookDetailsCollection.Find(x => x.Id == objectId).First();
                int blockedBooksCount = data.BlockBooks?.Count() ?? 0;
                if(blockedBooksCount <=  configDetails.BookBlockLimit )
                {
                    var IsISBNExists = bookDetails?.All(x => x.BlockBooks?.Any(y => y.ISBNNumber == blockedbookdetails.ISBNNumber) ?? false);
                    if (!IsISBNExists ?? false && blockedbookdetails.BookID != null)
                    {
                        blockedbookdetails.Created = DateTime.Now;
                        var update = Builders<BookDetails>.Update.Push("blockedBooks", blockedbookdetails).Inc("availableCopies", -1).Inc("blockedCopies", 1).Set("isbnNumber.$.occupied", true);
                        var result = await bookDetailsCollection.FindOneAndUpdateAsync(builders, update);
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
            catch(Exception e)
            {
                throw e;
            }
        }
        public  async Task<bool> UnBlockBooks(BlockBooks blockedbookdetails)
        {
            try
            {
                var database = LibManagementConnection.GetConnection();
                var userCollection = database.GetCollection<BookDetails>(CollectionConstant.Book_Collection);
                ObjectId objectId = ObjectId.Parse(blockedbookdetails.BookID);
                var builders = Builders<BookDetails>.Filter.And(Builders<BookDetails>.Filter.Eq(x => x.Id, objectId));
                var update = Builders<BookDetails>.Update.PullFilter("blockedBooks", Builders<BsonDocument>.Filter.Eq("isbnNumber", blockedbookdetails.ISBNNumber)).Inc("availableCopies", 1).Inc("blockedCopies", -1);
                var result = await userCollection.FindOneAndUpdateAsync(builders, update);
                if (result != null)
                {
                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<List<LatestBooks>> GetAllLatestBookDetails()
        {
            try
            {
                var result = await GetAllBooks();
                var latestBooks = (from books in result
                                   where books.ISBNNumber != null && books.ISBNNumber.Count > 0
                                   from item in books.ISBNNumber
                                   select new LatestBooks
                                   {
                                       BookID = books.Id.ToString(),
                                       Author = item.Author,
                                       Name = books.Name,
                                       Image = books.Image,
                                       Created = item.Created,
                                       Edition = item.Edition,
                                       Occupied = item.Occupied,
                                       TrackNo = item.TrackNo,
                                       PublishingYear = item.PublishingYear,Description=item.Description
                                   }).OrderByDescending(x => x.Created).Take(10).ToList<LatestBooks>();
                return latestBooks;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<List<ISBNNumber>> GetAllIsbnDetails()
        {
            try
            {
                var result = await GetAllBooks();
                var latestBooks = (from books in result
                                   where books.ISBNNumber != null && books.ISBNNumber.Count > 0
                                   from item in books.ISBNNumber
                                   select new ISBNNumber
                                   {
                                       BookID = books.Id.ToString(),
                                       Author = item.Author,
                                       BookName = books.Name,
                                       Created = item.Created,
                                       Edition = item.Edition,
                                       Occupied = item.Occupied,
                                       TrackNo = item.TrackNo,
                                       Description = item.Description,
                                       PublishingYear  = item.PublishingYear
                                   }).ToList<ISBNNumber>();
                return latestBooks;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> EditIsbnDetails(ISBNNumber iSBNNumber)
        {
            try
            {
                var database = LibManagementConnection.GetConnection();

                var booksDetailsCollection = database.GetCollection<BookDetails>(CollectionConstant.Book_Collection);
                ObjectId objectId = ObjectId.Parse(iSBNNumber.BookID);
                var data = await booksDetailsCollection.FindAsync(x => x.Id == objectId);
                var matchedDetails = await data.ToListAsync();
                var item = matchedDetails.FirstOrDefault(x => x.Id == objectId);
                var details = item.ISBNNumber.First(i => i.TrackNo == iSBNNumber.TrackNo);
                details.Author = iSBNNumber.Author;
                details.Description = iSBNNumber.Description;
                details.Edition = iSBNNumber.Edition;
                details.PublishingYear = iSBNNumber.PublishingYear;
                var result = booksDetailsCollection.ReplaceOne(c => c.Id == item.Id, item);
                if (result.IsModifiedCountAvailable)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Task<object> GetDashboardDetails()
        {
            throw new NotImplementedException();
        }
    }
}
