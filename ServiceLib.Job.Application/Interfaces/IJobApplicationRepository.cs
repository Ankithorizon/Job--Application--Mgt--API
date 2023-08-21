using EF.Core.Job.Application.Models;
using ServiceLib.Job.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ServiceLib.Job.Application.Interfaces
{
    public interface IJobApplicationRepository
    {
        Task<JobApplication> AddJobApp(JobApplication jobApplication);
        Task<IEnumerable<JobApplication>> GetAllJobApps();
        Task<List<string>> GetAppStatusTypes();
        Task<JobApplication> EditJobApp(JobApplicationEditVM jobApplication);
        Task<JobApplication> ViewJobApp(int jobAppId);
        Task<bool> DeleteJobApp(JobApplication jobApplication);
        Task<IEnumerable<AppStatusLog>> TrackJobAppStatus(int jobAppId);
        Task<bool> JobAppClosed(int jobApplicationId);
    }
}
