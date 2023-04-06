using EF.Core.Job.Application.Context;
using EF.Core.Job.Application.Models;
using ServiceLib.Job.Application.DTO;
using ServiceLib.Job.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceLib.Job.Application.Repositories
{
    public class JobResumeRepository : IJobResumeRepository
    {
        private readonly JobApplicationDBContext appDbContext;

        public JobResumeRepository(JobApplicationDBContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public bool JobAppClosed(int jobApplicationId)
        {
            var lastAppStatusLog = appDbContext.AppStatusLog
                                   .Where(x => x.JobApplicationId == jobApplicationId);
            if (lastAppStatusLog != null && lastAppStatusLog.Count() > 0)
            {
                var lastAppStatusLog_ = lastAppStatusLog.ToList().LastOrDefault();
                if (lastAppStatusLog_.AppStatus == AppStatusType.Closed)
                {
                    return true;
                }
            }
            return false;
        }

        public bool StoreResumeFile(JobResume jobResume)
        {
            try
            {
                // check for exception
                // throw new Exception();

                // key(column) : JobApplicationId 
                // Table : JobResumes
                // if record exist then override record
                // else just add record

                var jobResume_ = appDbContext.JobResumes
                                    .Where(x => x.JobApplicationId == jobResume.JobApplicationId).FirstOrDefault();
                if (jobResume_ != null)
                {
                    // override
                    jobResume_.FileName = jobResume.FileName;
                    jobResume_.FilePath = jobResume.FilePath;
                }
                else
                {
                    // add
                    var result = appDbContext.JobResumes.Add(jobResume);
                }
                appDbContext.SaveChanges();
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
