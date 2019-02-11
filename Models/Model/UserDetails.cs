﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class UserDetails
    {

        [BsonId]
        public ObjectId Id { get; set; }

        [Required]
        [BsonElement("firstName")]
        public string FirstName { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("middleName")]
        public string MiddleName { get; set; }

        [Required]
        [BsonElement("lastName")]
        public string LastName { get; set; }

        [Required]
        [BsonElement("username")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [BsonElement("email")]
        public string Email { get; set; }

        [Required]
        [BsonElement("dob")]
        public DateTime DateofBirth { get; set; }

        [Required]
        [BsonElement("gender")]
        public string Gender { get; set; }

        [BsonElement("created")]
        public DateTime Created { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("issuedBooks")]
        public IssueBooks IssuedBooks { get; set; }

        [BsonElement("lastUpdated")]
        public DateTime LastUpdated { get; set; }

        [Required]
        [BsonElement("roleType")]
        public RoleType RoleType { get; set; }

        [BsonElement("userId")]
        public string UserID { get; set; }

        [BsonIgnore]
        public string Password { get; set; }

    }
    public class IssueBooks
    {
        [BsonElement("isbnNumber")]
        public string ISBNNumber { get; set; }

        [BsonElement("returnDate")]
        public DateTime ReturnDate { get; set; }

        [BsonElement("issuedOn")]
        public DateTime IssuedOn { get; set; }
    }
}