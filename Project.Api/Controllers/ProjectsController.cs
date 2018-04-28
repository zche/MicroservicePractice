using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Project.Api.Application.Commands;
using MediatR;

namespace Project.Api.Controllers
{
    [Route("api/[controller]")]
    public class ProjectsController : BaseController
    {
        private IMediator _mediator;
        public ProjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("")]
        public async Task<IActionResult> CreateProject([FromBody]Domain.AggregatesModel.Project project)
        {
            var command = new CreateProjectCommand { Project = project };
            var proj = await _mediator.Send(command);
            return Ok(proj);
        }
        [HttpPut("view/{projectId}")]
        public async Task<IActionResult> ViewProject(int projectId)
        {
            var command = new ViewProjectCommand
            {
                UserId = UserIdentity.UserId,
                UserName = UserIdentity.Name,
                Avatar = UserIdentity.Avatar,
                ProjectId = projectId
            };
            await _mediator.Send(command);
            return Ok();
        }
        [HttpPut("join/{projectId}")]
        public async Task<IActionResult> JoinProject([FromBody]Domain.AggregatesModel.ProjectContributor contributor)
        {
            var command = new JoinProjectCommand { Contributor = contributor };
            await _mediator.Send(command);
            return Ok();
        }
    }
}
