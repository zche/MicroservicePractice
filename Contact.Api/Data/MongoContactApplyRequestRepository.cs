using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contact.Api.Models;
using MongoDB.Driver;
using Contact.Api.ElasticSearch;
using Nest;

namespace Contact.Api.Data
{
    public class MongoContactApplyRequestRepository : IContactApplyRequestRepository
    {
        private readonly ContactContext _context;
        private readonly ElasticClient _elasticClient;
        public MongoContactApplyRequestRepository(ContactContext context, ElasticClient elasticClient)
        {
            _context = context;
            _elasticClient = elasticClient;
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
            //await _elasticClient.IndexAsync(request, idx => idx.Index(ContactSearchConfig.ContactIndexName));
            return true;
        }

        public async Task<bool> ApproveAsync(int userId, int applicantId, CancellationToken cancellationToken)
        {
            var filter = Builders<ContactApplyRequest>.Filter.Where(r => r.UserId == userId && r.ApplicantId == applicantId);
            var update = Builders<ContactApplyRequest>.Update
                .Set(r => r.Approved, 1)
                .Set(r => r.HandledTime, DateTime.Now);
            var result = await _context.ContactApplyRequests.UpdateOneAsync(filter, update, null, cancellationToken);
            #region ES更新代码--先查询，在更新
            //var esEntities = await _elasticClient.SearchAsync<ContactApplyRequest>(s => s.From(0).Size(1)
            //    .Query(q => q.Match(m => m.Field(f => f.UserId).Query(userId.ToString()))));
            //DocumentPath<ContactApplyRequest> docPath = new DocumentPath<ContactApplyRequest>(esEntities.Hits.First().Id);
            //IUpdateRequest<ContactApplyRequest, object> request = new UpdateRequest<ContactApplyRequest, object>(docPath)
            //{
            //    Doc = new
            //    {
            //        Approved = 1,
            //        HandledTime = DateTime.Now
            //    }

            //};
            //await _elasticClient.UpdateAsync(request); 
            #endregion
            return result.MatchedCount == result.ModifiedCount;
        }

        public async Task<List<ContactApplyRequest>> GetRequestListAsync(int userId, CancellationToken cancellationToken)
        {
            var entities = await _context.ContactApplyRequests.FindAsync<ContactApplyRequest>(c => c.UserId == userId);
            //var esResults = await _elasticClient.SearchAsync<ContactApplyRequest>(s => s.From(0).Size(1)
            //.Query(q => q.Match(m => m.Field(f => f.UserId).Query(userId.ToString()))));
            //var esEntities = esResults.Documents.ToList();
            return await entities.ToListAsync(cancellationToken);
        }
    }
}
