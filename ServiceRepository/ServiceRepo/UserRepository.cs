using Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceRepository
{
    public class UserRepository : IUserRepository
    {
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

        public async Task<IEnumerable<object>> GetUserMailList()
        {

            try
            {
                var result = await GetAllUsers();
                var ss = (from user in result
                          where user.IssuedBooks != null
                          select new { user.Email, user.FullName,
                          issuedBooks = (from book in user.IssuedBooks
                                         where (book.ReturnDate.Date - DateTime.Now.Date).Days <= 1
                                         select book)});
                return ss;

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
                var logins = await userCollection.FindAsync(x => x.UserName == userdetails.UserName || x.UserID == userdetails.UserID || x.Email == userdetails.Email);
                var loginsList = await logins.ToListAsync();
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

    }
}
