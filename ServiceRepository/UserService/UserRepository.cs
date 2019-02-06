using Models;
using Models.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceRepository
{
    public class UserRepository
    {
        List<UserDetails> userDetails = new List<UserDetails>();
        public async Task GetAllUsers()
        {
            var database = LibManagementConnection.GetConnection();
            var todoTaskCollection = database.GetCollection<UserDetails>(CollectionConstant.User_Collection);
            var docs = await todoTaskCollection.FindAsync(new BsonDocument());
            await docs.ForEachAsync(doc => userDetails.Add(doc));
        }

        public async Task RegisterUser(LoginDetails userLoginDetails,UserDetails userdetails)
        {
            var database = LibManagementConnection.GetConnection();
            var userCollection = database.GetCollection<UserDetails>(CollectionConstant.User_Collection);
            var loginCollection = database.GetCollection<LoginDetails>(CollectionConstant.Login_Collection);
            await loginCollection.InsertOneAsync(userLoginDetails);
            await userCollection.InsertOneAsync(userdetails);
        }

    }
}
