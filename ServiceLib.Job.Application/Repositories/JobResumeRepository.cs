using EF.Core.Job.Application.Context;
using EF.Core.Job.Application.Models;
using Microsoft.EntityFrameworkCore;
using ServiceLib.Job.Application.DTO;
using ServiceLib.Job.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLib.Job.Application.Repositories
{
    public class JobResumeRepository : IJobResumeRepository
    {
        private readonly JobApplicationDBContext appDbContext;

        public JobResumeRepository(JobApplicationDBContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<bool> JobAppClosed(int jobApplicationId)
        {
            var lastAppStatusLog = appDbContext.AppStatusLog
                                   .Where(x => x.JobApplicationId == jobApplicationId);
            if (lastAppStatusLog != null && lastAppStatusLog.Count() > 0)
            {
                // var lastAppStatusLog_ = lastAppStatusLog.ToList().LastOrDefault();
                var lastAppStatusLog__ = await lastAppStatusLog.ToListAsync();
                var lastAppStatusLog_ = lastAppStatusLog__.LastOrDefault();
                if (lastAppStatusLog_.AppStatus == AppStatusType.Closed)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> StoreResumeFile(JobResume jobResume)
        {
            try
            {
                // check for exception
                // throw new Exception();

                // key(column) : JobApplicationId 
                // Table : JobResumes
                // if record exist then override record
                // else just add record

                var jobResume_ = await appDbContext.JobResumes
                                    .Where(x => x.JobApplicationId == jobResume.JobApplicationId).FirstOrDefaultAsync();
                if (jobResume_ != null)
                {
                    // override
                    jobResume_.FileName = jobResume.FileName;
                    jobResume_.FilePath = jobResume.FilePath;
                }
                else
                {
                    // add
                    var result = await appDbContext.JobResumes.AddAsync(jobResume);
                }
                await appDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string GetResumeFile(int jobApplicationId)
        {
            string resumeFileName = null;

            var jobResume = appDbContext.JobResumes
                                .Where(x => x.JobApplicationId == jobApplicationId).FirstOrDefault();
            if (jobResume != null)
                resumeFileName = jobResume.FileName;

            return resumeFileName;
        }
    }
}
