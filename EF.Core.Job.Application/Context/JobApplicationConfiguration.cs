using System;
using System.Collections.Generic;
using System.Text;
using EF.Core.Job.Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EF.Core.Job.Application.Context
{
    public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
    {
        public void Configure(EntityTypeBuilder<JobApplication> builder)
        {
            builder.HasKey(c => c.JobApplicationId);

            builder.Property(c => c.ContactPersonName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(c => c.ContactEmail)
              .IsRequired();
            builder.Property(c => c.City)
            .IsRequired();
            builder.Property(c => c.Province)
            .IsRequired();
            builder.Property(c => c.AppliedOn)
            .IsRequired();
        }
    }
}
