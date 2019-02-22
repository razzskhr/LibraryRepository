using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Model
{
   public class LatestBooks : ISBNNumber
   {
        public string Name { get; set; }
        public string Image { get; set; }
        public string PublishingYear { get; set; }
    }

}
   

