using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User.Api.Models;

namespace User.Api.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }
        public DbSet<AppUser> Users { get; set; }

        public DbSet<UserProperty> UserProperties { get; set; }

        public DbSet<UserTag> UserTags { get; set; }

        public DbSet<BPFile> BPFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>()
                .ToTable("Users")
                .HasKey(u => u.Id);


            modelBuilder.Entity<UserProperty>().Property(p => p.Key).HasMaxLength(100);
            modelBuilder.Entity<UserProperty>().Property(p => p.Value).HasMaxLength(100);

            modelBuilder.Entity<UserProperty>()
                .ToTable("UserProperties")
                .HasKey(u => new { u.AppUserId, u.Key, u.Value });

            modelBuilder.Entity<UserTag>().Property(p => p.Tag).HasMaxLength(100);
            modelBuilder.Entity<UserTag>()
                .ToTable("UserTags").
                HasKey(u => new { u.UserId, u.Tag });

            modelBuilder.Entity<BPFile>()
                .ToTable("BPFiles")
                .HasKey(b=>b.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
