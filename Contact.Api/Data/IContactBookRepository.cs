using Contact.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Contact.Api.Data
{
    public interface IContactBookRepository
    {
        Task<bool> UpdateContactTagsAsync(int userId, int contactId, List<string> tags, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<Models.Contact>> GetContactsAsync(int userId, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> UpdateContactBookInfoAsync(BaseUserInfo userInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> AddContactAysnc(int userId, BaseUserInfo userInfo, CancellationToken cancellationToken = default(CancellationToken));
    }
}
