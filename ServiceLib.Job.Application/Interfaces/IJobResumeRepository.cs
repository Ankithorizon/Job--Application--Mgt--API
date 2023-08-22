using System;
using System.Collections.Generic;
using System.Text;
using EF.Core.Job.Application.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ServiceLib.Job.Application.Interfaces
{
    public interface IJobResumeRepository
    {
        Task<bool> StoreResumeFile(JobResume jobResume);
        Task<bool> JobAppClosed(int jobApplicationId);
        string GetResumeFile(int jobApplicationId);

    }
}
