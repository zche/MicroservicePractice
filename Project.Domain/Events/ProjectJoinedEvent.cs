using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Project.Domain.AggregatesModel;

namespace Project.Domain.Events
{
    public class ProjectJoinedEvent : INotification
    {
        public string Company { get; set; }
        public string Introduction { get; set; }
        public ProjectContributor ProjectContributor { get; set; } = new ProjectContributor();
    }
}
