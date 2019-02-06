using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ServiceRepository
{
    public  class LibManagementConnection
    {
        private static IMongoDatabase database;
        public static IMongoDatabase GetConnection()
        {
            var connection = ConfigurationManager.ConnectionStrings["LibraryManagementConnection"]?.ToString();
            var databaseName = ConfigurationManager.ConnectionStrings["LibraryDatabase"]?.ToString();
            MongoClient client = new MongoClient(connection);
            database  = client.GetDatabase(databaseName);
            return database;
        }
    }
}