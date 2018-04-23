using Project.Domain.AggregatesModel;
using Project.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        public IUnitOfWork UnitOfWork => throw new NotImplementedException();

        public Task<Domain.AggregatesModel.Project> AddAsync(Domain.AggregatesModel.Project project)
        {
            throw new NotImplementedException();
        }

        public Task<Domain.AggregatesModel.Project> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Domain.AggregatesModel.Project> UpdateAsync(Domain.AggregatesModel.Project project)
        {
            throw new NotImplementedException();
        }
    }
}
