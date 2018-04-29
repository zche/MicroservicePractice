using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contact.Api.Models
{
    [BsonIgnoreExtraElements]
    public class ContactBook
    {
        public int UserId { get; set; }
        public List<Contact> Contacts { get; set; } = new List<Contact>();
    }
}
