using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Project.Domain.AggregatesModel;

namespace Project.Domain.Events
{
    public class ProjectJoinedEvent : INotification
    {
        public ProjectContributor ProjectContributor { get; set; } = new ProjectContributor();
    }
}
