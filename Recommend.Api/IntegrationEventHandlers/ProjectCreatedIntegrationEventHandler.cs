using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Recommend.Api.Data;
using Recommend.Api.IntegrationEvents;
using Recommend.Api.Models;
using Recommend.Api.Services;

namespace Recommend.Api.IntegrationEventHandlers
{
    public class ProjectCreatedIntegrationEventHandler : ICapSubscribe
    {
        private readonly RecommendContext _recommendContext;
        private readonly IUserService _userService;
        private readonly IContactService _contactService;
        public ProjectCreatedIntegrationEventHandler(RecommendContext recommendContext,
            IUserService userService, IContactService contactService)
        {
            _recommendContext = recommendContext;
            _userService = userService;
            _contactService = contactService;
        }

        [CapSubscribe("projectApi.project_created")]
        public async Task CreateRecommendServiceFromProject(ProjectCreatedIntegrationEvent @event)
        {
            var fromUser = await _userService.GetBaseUserInfoAsync(@event.UserId);
            var contacts = await _contactService.GetContactsByUserId(@event.UserId);
            foreach (var item in contacts)
            {
                var recommend = new ProjectRecommend
                {
                    FromUserId = @event.UserId,
                    Company = @event.Company,
                    Tags = @event.Tags,
                    ProjectId = @event.ProjectId,
                    ProjectAvatar = @event.ProjectAvatar,
                    FinancingStage = @event.FinancingStage,
                    RecommendedTime = DateTime.Now,
                    CreatedTime = @event.CreatedTime,
                    Introduction = @event.Introduction,
                    RecommendType = 1,
                    FromUserAvatar = fromUser.Avatar,
                    FromUserName = fromUser.Name,
                    UserId = item.UserId
                };
                await _recommendContext.ProjectRecommends.AddAsync(recommend);
            }
            await _recommendContext.SaveChangesAsync();
        }
    }
}
