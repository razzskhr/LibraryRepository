using Models;
using MongoDB.Bson;
using ServiceRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;

namespace ServiceRepository
{
    public class ConfigRepository : IConfigRepository
    {
        public async Task<ConfigDetails> GetConfigDetails()
        {

            try
            {
                ConfigDetails bookList = new ConfigDetails();
                var database = LibManagementConnection.GetConnection();
                var todoTaskCollection = database.GetCollection<ConfigDetails>(CollectionConstant.Config_Collection);
                var docs = await todoTaskCollection.Find(FilterDefinition<ConfigDetails>.Empty).SingleAsync();
                return docs ?? bookList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> UpdateConfigDetails(ConfigDetails configDetails)
        {
            try
            {
                if (configDetails.Id != null)
                {
                    var database = LibManagementConnection.GetConnection();
                    var todoTaskCollection = database.GetCollection<ConfigDetails>(CollectionConstant.Config_Collection);
                    ObjectId objectId = ObjectId.Parse(configDetails.ConfigId);
                    var builders = Builders<ConfigDetails>.Filter.And(Builders<ConfigDetails>.Filter.Where(x => x.Id == objectId));
                    var update = Builders<ConfigDetails>.Update.Set("bookIssueLimit", configDetails.BookIssueLimit)
                                                             .Set("bookBlockLimit", configDetails.BookBlockLimit)
                                                             .Set("returnDays", configDetails.ReturnDays);

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
            catch (Exception ex)
            {
                throw ex;
            }

        }


        //protected string GetElementName(string propertyName, Type classType)
        //{
        //    BsonClassMap classmap = BsonClassMap.LookupClassMap(classType);
        //    BsonMemberMap membermap = classmap.GetMemberMap(propertyName);

        //    return membermap.ElementName;
        //}

    }
}
