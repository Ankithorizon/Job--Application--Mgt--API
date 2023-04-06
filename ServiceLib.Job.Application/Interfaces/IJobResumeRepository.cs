using System;
using System.Collections.Generic;
using System.Text;
using EF.Core.Job.Application.Models;

namespace ServiceLib.Job.Application.Interfaces
{
    public interface IJobResumeRepository
    {
        bool StoreResumeFile(JobResume jobResume);
        bool JobAppClosed(int jobApplicationId);
        string GetResumeFile(int jobApplicationId);

    }
}
