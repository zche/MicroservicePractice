using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Project.Api.Application.Commands;
using MediatR;
using Project.Api.Application.Services;
using Project.Api.Application.Queries;

namespace Project.Api.Controllers
{
    [Route("api/[controller]")]
    public class ProjectsController : BaseController
    {
        private IMediator _mediator;
        private IRecommend _recommand;
        private IProjectQueries _projectQueries;
        public ProjectsController(IMediator mediator, IRecommend recommand, IProjectQueries projectQueries)
        {
            _mediator = mediator;
            _recommand = recommand;
            _projectQueries = projectQueries;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await _projectQueries.GetProjectsByUserIdAsync(UserIdentity.UserId);
            return Ok(projects);
        }

        [HttpGet("my/{projectId}")]
        public async Task<IActionResult> GetMyProjectDetail(int projectId)
        {
            var detail = await _projectQueries.GetProjectDetail(projectId);
            if (detail.UserId == UserIdentity.UserId)
                return Ok(detail);
            else
                return BadRequest("无权查看该项目");
        }

        [HttpGet("recommend/{projectId}")]
        public async Task<IActionResult> GetRecommendProjectDetail(int projectId)
        {
            if (await _recommand.IsRecommendedProject(projectId, UserIdentity.UserId))
            {
                var detail = await _projectQueries.GetProjectDetail(projectId);
                return Ok(detail);
            }
            else
                return BadRequest("无权查看该项目");
        }


        [HttpPost("")]
        public async Task<IActionResult> CreateProject([FromBody]Domain.AggregatesModel.Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(this.CreateProject));
            project.UserId = UserIdentity.UserId;
            var command = new CreateProjectCommand { Project = project };
            var proj = await _mediator.Send(command);
            return Ok(proj);
        }
        [HttpPut("view/{projectId}")]
        public async Task<IActionResult> ViewProject(int projectId)
        {

            if (!(await _recommand.IsRecommendedProject(projectId, UserIdentity.UserId)))
            {
                return BadRequest("没有查看该项目的权限");
            }
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
            if (!(await _recommand.IsRecommendedProject(contributor.ProjectId, UserIdentity.UserId)))
            {
                return BadRequest("没有加入该项目的权限");
            }
            var command = new JoinProjectCommand { Contributor = contributor };
            await _mediator.Send(command);
            return Ok();
        }
    }
}
