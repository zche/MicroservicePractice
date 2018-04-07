using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contact.Api.Dtos;
using Contact.Api.Models;
using MongoDB.Driver;

namespace Contact.Api.Data
{
    public class MongoContactBookRepository : IContactBookRepository
    {
        private readonly ContactContext _context;
        public MongoContactBookRepository(ContactContext context)
        {
            _context = context;
        }

        public async Task<bool> AddContactAysnc(int userId, BaseUserInfo userInfo, CancellationToken cancellationToken)
        {
            if (_context.ContactBooks.Count(c => c.UserId == userId) == 0)
            {
                await _context.ContactBooks.InsertOneAsync(new ContactBook { UserId = userId });
            }

            var filter = Builders<ContactBook>.Filter.Eq(c => c.UserId, userId);
            var update = Builders<ContactBook>.Update.AddToSet(c => c.Contacts, new Models.Contact
            {
                UserId = userInfo.UserId,
                Avatar = userInfo.Avatar,
                Company = userInfo.Company,
                Name = userInfo.Name,
                Title = userInfo.Title
            });
            var result = await _context.ContactBooks.UpdateOneAsync(filter, update, null, cancellationToken);
            return result.MatchedCount == result.ModifiedCount;
        }

        public async Task<List<Models.Contact>> GetContactsAsync(int userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var contactBook = await (await _context.ContactBooks.FindAsync(c => c.UserId == userId)).FirstOrDefaultAsync();
            if (contactBook != null)
                return contactBook.Contacts;
            return new List<Models.Contact>();
        }

        public async Task<bool> UpdateContactBookInfoAsync(BaseUserInfo userInfo, CancellationToken cancellationToken)
        {
            var contactBook = await (await _context.ContactBooks.FindAsync(p => p.UserId == userInfo.UserId, null, cancellationToken)).FirstOrDefaultAsync();
            if (contactBook == null) return true;
            var contactIds = contactBook.Contacts.Select(c => c.UserId);
            var filter = Builders<ContactBook>.Filter.And(
                Builders<ContactBook>.Filter.In(c => c.UserId, contactIds),
                Builders<ContactBook>.Filter.ElemMatch(c => c.Contacts, item => item.UserId == userInfo.UserId)
                );
            var update = Builders<ContactBook>.Update
                .Set("Contacts.$.Name", userInfo.Name)
                .Set("Contacts.$.Avatar", userInfo.Avatar)
                .Set("Contacts.$.Company", userInfo.Company)
                .Set("Contacts.$.Title", userInfo.Title);

            var updateResult = _context.ContactBooks.UpdateMany(filter, update);
            return updateResult.MatchedCount == updateResult.ModifiedCount;
        }

        public async Task<bool> UpdateContactTagsAsync(int userId, int contactId, List<string> tags, CancellationToken cancellationToken)
        {
            var filter = Builders<ContactBook>.Filter.And(
             Builders<ContactBook>.Filter.Eq(c => c.UserId, userId),
             Builders<ContactBook>.Filter.Eq("Contacts.UserId",contactId)
             );
            var update = Builders<ContactBook>.Update
                .Set("Contacts.$.Tags", tags);
            var result = await _context.ContactBooks.UpdateOneAsync(filter,update,null, cancellationToken);
            return result.MatchedCount == result.ModifiedCount;
        }
    }
}
