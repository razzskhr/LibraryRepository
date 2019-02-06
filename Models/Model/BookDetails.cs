using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class BookDetails
    {
        [BsonId]
        public ObjectId Id { get; set; }


        [BsonElement("isbnNumber")]
        public ISBNNumber ISBNNumber { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("author")]
        public string Author { get; set; }

        [BsonElement("publishingYear")]
        public string PublishingYear { get; set; }

        [BsonElement("numberOfCopies")]
        public int NumberOfCopies { get; set; }

        [BsonElement("availableCopies")]
        public int AvailableCopies { get; set; }

        [BsonElement("blockedCopies")]
        public int BlockedCopies { get; set; }

        [BsonElement("image")]
        public string Image { get; set; }

        [BsonElement("notification")]
        public List<NotificationDetails> Notification { get; set; }

        [BsonElement("created")]
        public DateTime Created { get; set; }

        [BsonElement("lastUpdated")]
        public DateTime LastUpdated { get; set; }

    }
    public class ISBNNumber
    {
        [BsonElement("trackNo")]
        public string TrackNo { get; set; }

        [BsonElement("occupied")]
        public bool Occupied { get; set; }

        [BsonElement("edition")]
        public string Edition { get; set; }

        [BsonElement("requestForBlock")]
        public string RequestForBlock { get; set; }
    }

    public class NotificationDetails
    {
        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("userID")]
        public string UserID { get; set; }
    }
}
