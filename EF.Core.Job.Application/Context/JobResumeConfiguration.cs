using System;
using System.Collections.Generic;
using System.Text;
using EF.Core.Job.Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EF.Core.Job.Application.Context
{
    public class JobResumeConfiguration : IEntityTypeConfiguration<JobResume>
    {
        public void Configure(EntityTypeBuilder<JobResume> builder)
        {
            builder.HasKey(c => c.JobResumeId);

            builder.Property(c => c.FileName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(c => c.FilePath)
              .IsRequired();
        }
    }
}
