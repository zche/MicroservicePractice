using Microsoft.EntityFrameworkCore;
using Recommend.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recommend.Api.Data
{
    public class RecommendContext : DbContext
    {
        public RecommendContext(DbContextOptions<RecommendContext> options) : base(options)
        {

        }
        public DbSet<ProjectRecommend> ProjectRecommends { get; set; }
        public DbSet<ProjectReferenceUser> ProjectReferenceUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectRecommend>().ToTable("ProjectRecommends").HasKey(p=>p.Id);
            modelBuilder.Entity<ProjectReferenceUser>().ToTable("ProjectReferenceUsers").HasKey(p => p.Id);
            base.OnModelCreating(modelBuilder);
        }
    }
}
