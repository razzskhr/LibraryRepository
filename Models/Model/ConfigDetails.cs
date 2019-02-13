using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class ConfigDetails
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [Required]
        [Range(0,10)]
        [BsonElement("bookIssueLimit")]
        public int BookIssueLimit { get; set; }


        [Required]
        [Range(0,10)]
        [BsonElement("bookBlockLimit")]
        public int BookBlockLimit { get; set; }

        [Required]
        [Range(0,30)]
        [BsonElement("returnDays")]
        public int ReturnDays { get; set; }

        [BsonIgnore]
        public string ConfigId { get; set; }
    }
}