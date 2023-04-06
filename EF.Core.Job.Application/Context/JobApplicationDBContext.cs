using EF.Core.Job.Application.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EF.Core.Job.Application.Context
{
    public class JobApplicationDBContext : DbContext
    {
        public JobApplicationDBContext(DbContextOptions<JobApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<JobResume> JobResumes { get; set; }
        public DbSet<AppStatusLog> AppStatusLog { get; set; }
        public DbSet<UserResumeCreate> UserResumeCreate { get; set; }
        public DbSet<UserResumeEmail> UserResumeEmail { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new JobApplicationConfiguration());
            modelBuilder.ApplyConfiguration(new JobResumeConfiguration());

            modelBuilder.Entity<JobResume>()
             .HasOne<JobApplication>(p => p.JobApplication)
             .WithOne(s => s.JobResume);

            modelBuilder.Entity<AppStatusLog>()
            .HasOne<JobApplication>(s => s.JobApplication)
            .WithMany(g => g.AppStatusLog)
            .HasForeignKey(s => s.JobApplicationId);
        }
    }
}
