using MediatR;
using Project.Domain.AggregatesModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Project.Api.Application.Commands
{
    public class ViewProjectCommandHandler : IRequestHandler<ViewProjectCommand<bool>,bool>
    {
        private IProjectRepository _projectRepository;
        public ViewProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<bool> Handle(ViewProjectCommand<bool> request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetAsync(request.ProjectId);
            if (project == null)
            {
                throw new Domain.Exceptions.ProjectDomainException($"Project not found:{request.ProjectId}");
            }
            project.AddViewer(new ProjectViewer { UserId = request.UserId, UserName = request.UserName, Avatar = request.Avatar });
            var result = await _projectRepository.UnitOfWork.SaveEntitiesAsync();
            return result;
        }
    }
}
