using Common.EncryptionRepository;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServiceRepository
{
    public class UserRepository : IUserRepository
    {
        private IPasswordRepository passwordRepository;

        public UserRepository(IPasswordRepository passwordRepository)
        {
            this.passwordRepository = passwordRepository;
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
            List<string> mailList = new List<string>();
            try
            {
                var database = LibManagementConnection.GetConnection();
                var todoTaskCollection = database.GetCollection<UserDetails>(CollectionConstant.User_Collection);
                var docs = await todoTaskCollection.FindAsync(item => item.IssuedBooks.Any(x => (x.ReturnDate.Date - DateTime.Now.Date).Days <= 1));
                await docs.ForEachAsync(item => mailList.Add(item.Email));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return mailList;
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
                string encryptedPassword=await passwordRepository.GetEncryptedPassword(userLoginDetails.Password);
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
                if(logins.ToListAsync().Result.Count > 0)
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
            catch(Exception e)
            {
                //await clientSession.AbortTransactionAsync();
                throw e;
            }
        }
    }
}
