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

        public async Task<bool> RegisterUser(LoginDetails userLoginDetails, UserDetails userdetails)
        {
            try
            {                
                var database = LibManagementConnection.GetConnection();
                var userCollection = database.GetCollection<UserDetails>(CollectionConstant.User_Collection);
                var loginCollection = database.GetCollection<LoginDetails>(CollectionConstant.Login_Collection);
                var logins = await userCollection.FindAsync(x => x.UserName == userdetails.UserName || x.UserID == userdetails.UserID || x.Email == userdetails.Email);
                var loginsList = await logins.ToListAsync();
                if(loginsList?.Count == 0)
                {
                    await loginCollection.InsertOneAsync(userLoginDetails);
                    await userCollection.InsertOneAsync(userdetails);
                    return true;
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

    }
}
