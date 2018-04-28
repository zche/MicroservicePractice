using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;
using MediatR;
using Project.Domain.Events;
using Project.Api.IntegrationEvents;

namespace Project.Api.DomainEventHandlers
{
    public class ProjectCreatedDomainEventHandler : INotificationHandler<ProjectCreatedEvent>
    {
        private ICapPublisher _capPublisher;
        public ProjectCreatedDomainEventHandler(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }
        public async Task Handle(ProjectCreatedEvent notification, CancellationToken cancellationToken)
        {
            var @event = new ProjectCreatedIntegrationEvent {
                ProjectId =notification.Project.Id,
                CreatedTime=DateTime.Now,
                UserId=notification.Project.UserId
            };
            await _capPublisher.PublishAsync("projectApi.project_created",@event);
        }
    }
}
