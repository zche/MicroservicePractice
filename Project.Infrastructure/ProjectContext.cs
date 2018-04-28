using Microsoft.EntityFrameworkCore;
using Project.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Project.Infrastructure.EntityConfigurations;
using Project.Domain.AggregatesModel;

namespace Project.Infrastructure
{
    public class ProjectContext : DbContext, IUnitOfWork
    {
        private IMediator _mediator;
        public ProjectContext(DbContextOptions<ProjectContext> options, IMediator mediator) :base(options)
        {
            _mediator = mediator;
        }

        public DbSet<Domain.AggregatesModel.Project> Projects { get; set; }
        public DbSet<ProjectContributor> ProjectContributors { get; set; }
        public DbSet<ProjectProperty> ProjectPropertys { get; set; }
        public DbSet<ProjectViewer> ProjectViewers { get; set; }
        public DbSet<ProjectVisibleRule> ProjectVisibleRules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProjectEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectContributorEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectPropertyEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectViewerEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectVisibleRuleEntityTypeConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await _mediator.DispatchDomainEventsAsync(this);
            await base.SaveChangesAsync();
            return true;
        }
    }
}
