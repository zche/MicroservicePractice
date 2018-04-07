using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contact.Api.Models;
using MongoDB.Driver;

namespace Contact.Api.Data
{
    public class MongoContactApplyRequestRepository : IContactApplyRequestRepository
    {
        private readonly ContactContext _context;
        public MongoContactApplyRequestRepository(ContactContext context)
        {
            _context = context;
        }
        public async Task<bool> AddRequestAsync(ContactApplyRequest request, CancellationToken cancellationToken)
        {
            var filter = Builders<ContactApplyRequest>.Filter.Where(r => r.UserId == request.UserId && r.ApplicantId == request.ApplicantId);
            if ((await _context.ContactApplyRequests.CountAsync(filter)) > 0)
            {
                var updateResult = await _context.ContactApplyRequests.ReplaceOneAsync(c => c.UserId == request.UserId && c.ApplicantId == request.ApplicantId, request);
                return updateResult.ModifiedCount > 0;
            }
            await _context.ContactApplyRequests.InsertOneAsync(request, null, cancellationToken);
            return true;
        }

        public async Task<bool> ApproveAsync(int userId, int applicantId, CancellationToken cancellationToken)
        {
            var filter = Builders<ContactApplyRequest>.Filter.Where(r => r.UserId == userId && r.ApplicantId == applicantId);
            var update = Builders<ContactApplyRequest>.Update
                .Set(r => r.Approved, 1)
                .Set(r => r.HandledTime, DateTime.Now);
            var result = await _context.ContactApplyRequests.UpdateOneAsync(filter, update, null, cancellationToken);
            return result.MatchedCount == result.ModifiedCount;
        }

        public async Task<List<ContactApplyRequest>> GetRequestListAsync(int userId, CancellationToken cancellationToken)
        {
            var entities = await _context.ContactApplyRequests.FindAsync<ContactApplyRequest>(c => c.UserId == userId);
            return await entities.ToListAsync(cancellationToken);
        }
    }
}
