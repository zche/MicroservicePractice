﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Project.Api.Application.Commands
{
    public class JoinProjectCommand : IRequest
    {
        public Domain.AggregatesModel.ProjectContributor Contributor { get; set; }
}
}
