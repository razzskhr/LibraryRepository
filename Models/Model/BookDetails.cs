﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public List<ISBNNumber> ISBNNumber { get; set; }

        [Required]
        [BsonElement("name")]
        public string Name { get; set; }

        [Required]
        [BsonElement("author")]
        public string Author { get; set; }

        [Required]
        [BsonElement("publishingYear")]
        public string PublishingYear { get; set; }

        [BsonElement("numberOfCopies")]
        public long NumberOfCopies { get; set; }

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
        
        [BsonIgnore]
        public string id { get; set; }

    }
    public class ISBNNumber
    {
        [BsonIgnore]
        public string id { get; set; }

        [Required]
        [BsonElement("trackNo")]
        public string TrackNo { get; set; }

        [BsonElement("occupied")]
        public bool Occupied { get; set; }

        [Required]
        [BsonElement("edition")]
        public string Edition { get; set; }

        [BsonElement("requestForBlock")]
        public string RequestForBlock { get; set; }

        [BsonElement("created")]
        public DateTime Created { get; set; }
    }

    public class NotificationDetails
    {
        [EmailAddress]
        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("userID")]
        public string UserID { get; set; }
    }
}
