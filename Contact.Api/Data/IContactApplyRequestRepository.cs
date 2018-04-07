using Contact.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Contact.Api.Data
{
    public interface IContactApplyRequestRepository
    {
        Task<bool> AddRequestAsync(ContactApplyRequest request, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> ApproveAsync(int userId, int applicantId, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<ContactApplyRequest>> GetRequestListAsync(int userId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
