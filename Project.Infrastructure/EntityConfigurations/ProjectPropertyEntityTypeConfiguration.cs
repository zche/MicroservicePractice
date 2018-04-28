using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.AggregatesModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Infrastructure.EntityConfigurations
{
    public class ProjectPropertyEntityTypeConfiguration : IEntityTypeConfiguration<ProjectProperty>
    {
        public void Configure(EntityTypeBuilder<ProjectProperty> builder)
        {
            builder.Property(p => p.Key).HasMaxLength(100);
            builder.Property(p => p.Value).HasMaxLength(100);
            builder.ToTable("ProjectPropertys").HasKey(p => new { p.ProjectId, p.Key, p.Value });
        }
    }
}
