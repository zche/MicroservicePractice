using Project.Domain.AggregatesModel;
using Project.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Project.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ProjectContext _context;
        public IUnitOfWork UnitOfWork => _context;
        public ProjectRepository(ProjectContext context)
        {
            _context = context;
        }

        public async Task<Domain.AggregatesModel.Project> AddAsync(Domain.AggregatesModel.Project project)
        {
            if (project.IsTransient())
            {
                var result = await _context.AddAsync(project);
                return result.Entity;
            }
            return project;
        }

        public async Task<Domain.AggregatesModel.Project> GetAsync(int id)
        {
            return await _context.Projects.Include(p => p.Properties)
                .Include(p => p.Viewers)
                .Include(p => p.Contributors)
                .Include(p => p.VisibleRule)
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        public void UpdateAsync(Domain.AggregatesModel.Project project) => _context.Update(project);
    }
}
