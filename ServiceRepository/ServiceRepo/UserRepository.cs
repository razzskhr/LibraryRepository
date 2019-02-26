using Common.EncryptionRepository;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServiceRepository
{
    public class UserRepository : IUserRepository
    {
        private IPasswordRepository passwordRepository;
        private IConfigRepository configRepository;

        public UserRepository(IPasswordRepository passwordRepository, IConfigRepository configRepository)
        {
            this.passwordRepository = passwordRepository;
            this.configRepository = configRepository;
        }
        public async Task<List<UserDetails>> GetAllUsers()
        {
            try
            {
                List<UserDetails> userDetails = new List<UserDetails>();
                var database = LibManagementConnection.GetConnection();
                var todoTaskCollection = database.GetCollection<UserDetails>(CollectionConstant.User_Collection);
                var docs = await todoTaskCollection.FindAsync(new BsonDocument());
                await docs.ForEachAsync(doc => userDetails.Add(doc));
                return userDetails;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<string>> GetUserMailList()
        {

            try
            {
                var result = await GetAllUsers();
                var issuedList = result.Where(x => x.IssuedBooks != null && x.IssuedBooks.Any());
                var list = (from user in issuedList

                            from book in user.IssuedBooks
                            where (book.ReturnDate.Date - DateTime.Now.Date).Days <= 1
                            select user.Email).ToList();
                //select new UserDetails
                //{
                //    Id =user.Id,
                //    Email = user.Email,
                //    FirstName = user.FirstName,
                //    MiddleName = user.MiddleName,
                //    LastName = user.LastName,
                //    IssuedBooks = (from book in user.IssuedBooks
                //                   where (book.ReturnDate.Date - DateTime.Now.Date).Days <= 1
                //                   select book).ToList()
                //});
                return list;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public UserDetails GetLoggedInUserDetails(string username)
        {
            try
            {
                List<UserDetails> userDetails = new List<UserDetails>();
                var database = LibManagementConnection.GetConnection();
                var todoTaskCollection = database.GetCollection<UserDetails>(CollectionConstant.User_Collection);
                var user = todoTaskCollection.Find(x => x.UserName == username).FirstOrDefault();
                return user;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<Response<string>> RegisterUser(LoginDetails userLoginDetails, UserDetails userdetails)
        {
            //IClientSessionHandle session = null;
            try
            {
                var database = LibManagementConnection.GetConnection();
                var userCollection = database.GetCollection<UserDetails>(CollectionConstant.User_Collection);
                var loginCollection = database.GetCollection<LoginDetails>(CollectionConstant.Login_Collection);
                var logins = await loginCollection.FindAsync(x => x.UserName.ToLower() == userdetails.UserName.ToLower());
                var loginsList = await logins.ToListAsync();
                string encryptedPassword = await passwordRepository.GetEncryptedPassword(userLoginDetails.Password);
                userLoginDetails.Password = encryptedPassword;
                if (loginsList?.Count == 0)
                {
                    //session = await database.Client.StartSessionAsync();
                    //session.StartTransaction();
                    await loginCollection.InsertOneAsync(userLoginDetails);
                    await userCollection.InsertOneAsync(userdetails);
                    //await session.CommitTransactionAsync();
                    return new Response<string>() { StatusCode = System.Net.HttpStatusCode.OK };

                }
                else
                {
                    return new Response<string>() { StatusCode = System.Net.HttpStatusCode.BadRequest, Message = "User Already Exists" };
                }

            }
            catch (Exception ex)
            {
                //await session.AbortTransactionAsync();
                throw ex;
            }
        }

        public async Task<bool> UpdatePassword(LoginDetails userLoginDetails)
        {
            //IClientSessionHandle clientSession = null;
            try
            {
                var oldPassword = await passwordRepository.GetEncryptedPassword(userLoginDetails.OldPassword);
                var newPassword = await passwordRepository.GetEncryptedPassword(userLoginDetails.Password);
                var database = LibManagementConnection.GetConnection();
                //clientSession = await database.Client.StartSessionAsync();
                var loginCollection = database.GetCollection<LoginDetails>(CollectionConstant.Login_Collection);
                var logins = await loginCollection.FindAsync(x => x.UserName == userLoginDetails.UserName);
                if (logins.ToListAsync().Result.Count > 0)
                {
                    //clientSession.StartTransaction();
                    userLoginDetails.Password = newPassword;
                    var builders = Builders<LoginDetails>.Filter.And(Builders<LoginDetails>.Filter.Where(x => x.UserName == userLoginDetails.UserName));
                    var update = Builders<LoginDetails>.Update.Set("password", userLoginDetails.Password);
                    var result = await loginCollection.FindOneAndUpdateAsync(builders, update);
                    //await clientSession.CommitTransactionAsync();
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
            catch (Exception e)
            {
                //await clientSession.AbortTransactionAsync();
                throw e;
            }
        }
        public async Task<Response<string>> InsertImageFileName(string userName, string image)
        {
            try
            {
                var database = LibManagementConnection.GetConnection();
                var userCollection = database.GetCollection<UserDetails>(CollectionConstant.User_Collection);
                var builders = Builders<UserDetails>.Filter.And(Builders<UserDetails>.Filter.Where(x => x.UserName == userName));
                var update = Builders<UserDetails>.Update.Set("image", image);

                var result = await userCollection.FindOneAndUpdateAsync(builders, update);
                if (result != null)
                {
                    return new Response<string>() { StatusCode = HttpStatusCode.OK };
                }
                else
                {
                    return new Response<string>() { StatusCode = HttpStatusCode.BadRequest };
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RemoveBlockedBookList()
        {
            var userList = await GetAllUsers();

            return true;
        }

        public Task<bool> RemoveAllBlockedBookList()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteUser(string userName)
        {
            bool deleteResult = false;
            //IClientSessionHandle session = null;
            try
            {
                var database = LibManagementConnection.GetConnection();
                // session = database.Client.StartSession();
                //session.StartTransaction();
                var todoTaskCollection = database.GetCollection<UserDetails>(CollectionConstant.User_Collection);
                var result = await todoTaskCollection.FindOneAndDeleteAsync(Builders<UserDetails>.Filter.Eq("username", userName));

                if (result != null)
                {
                    var loginCollection = database.GetCollection<LoginDetails>(CollectionConstant.Login_Collection);
                    var deleteRes = await loginCollection.FindOneAndDeleteAsync(Builders<LoginDetails>.Filter.Eq("username", userName));

                    if (deleteRes != null)
                    {
                        deleteResult = true;
                    }
                }
                //session.CommitTransaction();
                return deleteResult;
            }
            catch (Exception ex)
            {
                //session?.AbortTransaction();
                throw ex;
            }
        }

        public async Task<bool> UserReturnBooks(IssueBooks isbnDetails)
        {
            try
            {
                var database = LibManagementConnection.GetConnection();
                var userCollection = database.GetCollection<UserDetails>(CollectionConstant.User_Collection);
                ObjectId objectId = ObjectId.Parse(isbnDetails.BookID);
                var builders = Builders<UserDetails>.Filter.And(Builders<UserDetails>.Filter.Eq(x => x.Id, objectId));
                var update = Builders<UserDetails>.Update.PullFilter("issuedBooks", Builders<BsonDocument>.Filter.Eq("isbnNumber", isbnDetails.ISBNNumber));
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
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> IssueBooksToUser(IssueBooks isbnDetails)
        {
            try
            {
                var database = LibManagementConnection.GetConnection();
                var userCollection = database.GetCollection<UserDetails>(CollectionConstant.User_Collection);
                ObjectId objectId = ObjectId.Parse(isbnDetails.BookID);
                var builders = Builders<UserDetails>.Filter.And(Builders<UserDetails>.Filter.Eq(x => x.Id, objectId));
                var bookDetails = await userCollection.Find(builders).ToListAsync();
                var configDetails = await configRepository.GetConfigDetails();
                var data = userCollection.Find(x => x.Id == objectId).First();
                int issuedBooksCount = data.IssuedBooks.Count();
                if (issuedBooksCount <= configDetails.BookIssueLimit)
                {
                    var IsISBNExists = bookDetails?.Any(x => x.IssuedBooks?.Any(y => y.ISBNNumber == isbnDetails.ISBNNumber) ?? false);
                    if (!IsISBNExists ?? false && isbnDetails.BookID != null)
                    {
                        isbnDetails.IssuedOn = DateTime.Now;
                        isbnDetails.ReturnDate = isbnDetails.IssuedOn.AddDays(15);
                        var update = Builders<UserDetails>.Update.Push("issuedBooks", isbnDetails);
                        var result = await userCollection.FindOneAndUpdateAsync(builders, update);
                    }
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



        public async Task<bool> UnBlockBooks(BlockBooks blockedbookdetails)
        {
            try
            {
                var database = LibManagementConnection.GetConnection();
                var userCollection = database.GetCollection<UserDetails>(CollectionConstant.User_Collection);
                ObjectId objectId = ObjectId.Parse(blockedbookdetails.BookID);
                var builders = Builders<UserDetails>.Filter.And(Builders<UserDetails>.Filter.Eq(x => x.Id, objectId));
                var update = Builders<UserDetails>.Update.PullFilter("blockedBooks", Builders<BsonDocument>.Filter.Eq("isbnNumber", blockedbookdetails.ISBNNumber));
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
            catch (Exception e)
            {
                throw e;
            }

        }

        public List<IssueBooks> GetAllIssuedbooksToUser(string userId)
        {
            try
            {
                var database = LibManagementConnection.GetConnection();
                var userCollection = database.GetCollection<UserDetails>(CollectionConstant.User_Collection);
                var user = userCollection.Find(x => x.UserName == userId).First();
                var issuedbooks = user.IssuedBooks.OrderBy(x => x.ReturnDate).ToList();
                return issuedbooks;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<bool> CheckUserNameAvailability(string userName)
        {
            try
            {
                var database = LibManagementConnection.GetConnection();
                var userCollection = database.GetCollection<UserDetails>(CollectionConstant.Login_Collection);
                var user = await userCollection.FindAsync(x => x.UserName.ToUpper() == userName.ToUpper());
                var result=await user.ToListAsync();
                if (result?.Count > 0)
                {
                    return false;
                }
                else
                {
                    return true; 
                }
            }
            catch(Exception e)
            {
                throw e;
            }


        }

        public async Task<bool> UpdateUserDetails(UserDetails userDetails)
        {
            try
            {
                if (userDetails != null)
                {
                    var database = LibManagementConnection.GetConnection();
                    //session = database.Client.StartSession();
                    //session.StartTransaction();
                    var todoTaskCollection = database.GetCollection<UserDetails>(CollectionConstant.User_Collection);
                    var builders = Builders<UserDetails>.Filter.And(Builders<UserDetails>.Filter.Where(x => x.UserName == userDetails.UserName));
                    var update = Builders<UserDetails>.Update.Set("firstName", userDetails.FirstName).Set("middleName", userDetails.MiddleName)
                                                             .Set("lastName", userDetails.LastName).Set("image", userDetails.Image).Set("lastUpdated", System.DateTime.UtcNow)
                                                             .Set("dob", userDetails.DateofBirth).Set("phoneNumber", userDetails.PhoneNumber);

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
    }
}
