using Contact.Api.Data;
using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Contact.Api.IntegrationEvents
{
    public class UserProfileChangedEventHandler:ICapSubscribe
    {
        private IContactBookRepository _contactBookRepository;
        public UserProfileChangedEventHandler(IContactBookRepository contactBookRepository)
        {
            _contactBookRepository = contactBookRepository;
        }
        [CapSubscribe("userapi.user_profile_changed")]
        public async Task UpdateContactInfo(UserProfileChangedEvent @event)
        {
            await _contactBookRepository.UpdateContactBookInfoAsync(
                new Dtos.BaseUserInfo {
                    Avatar = @event.Avatar,
                    Company=@event.Company,
                    Name=@event.Name,
                    Title=@event.Title,
                    UserId=@event.UserId
                });
        }
    }
}
