using Contact.Api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contact.Api.Data
{
    public class ContactContext
    {
        public readonly IMongoDatabase _db;

        public ContactContext(IOptions<MongoDBSetting> options)

        {
            var permissionSystem =
                MongoCredential.CreateCredential(options.Value.DataBase, options.Value.UserName,
                    options.Value.Password);
            var services = new List<MongoServerAddress>();
            foreach (var item in options.Value.Services)
            {
                services.Add(new MongoServerAddress(item.Host, item.Port));
            }
            var settings = new MongoClientSettings
            {
                Credential = permissionSystem ,
                Servers = services
            };


            var _mongoClient = new MongoClient(settings);
            _db = _mongoClient.GetDatabase(options.Value.DataBase);
        }

        public IMongoCollection<ContactApplyRequest> ContactApplyRequests => _db.GetCollection<ContactApplyRequest>("ContactApplyRequests");

        public IMongoCollection<Models.Contact> Contacts => _db.GetCollection<Models.Contact>("Contacts");

        public IMongoCollection<ContactBook> ContactBooks => _db.GetCollection<ContactBook>("ContactBooks");
    }
}
