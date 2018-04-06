using Contact.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contact.Api.Data
{
    public interface IContactApplyRequestRepository
    {
        Task<bool> AddRequestAsync(ContactApplyRequest request);

        Task<bool> ApproveAsync(int applicantId);

        Task<List<Contact.Api.Models.Contact>> GetRequestListAsync(int userId);
    }
}
