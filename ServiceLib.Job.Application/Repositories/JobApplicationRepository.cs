﻿using EF.Core.Job.Application.Context;
using EF.Core.Job.Application.Models;
using ServiceLib.Job.Application.DTO;
using ServiceLib.Job.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace ServiceLib.Job.Application.Repositories
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly JobApplicationDBContext appDbContext;

        public JobApplicationRepository(JobApplicationDBContext appDbContext)
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

        // ef-core transaction
        public async Task<JobApplication> AddJobApp(JobApplication jobApplication)
        {
            using var transaction = appDbContext.Database.BeginTransaction();
            try
            {
                // 1)
                var result = await appDbContext.JobApplications.AddAsync(jobApplication);
                await appDbContext.SaveChangesAsync();

                // throw new Exception();

                // 2)
                AppStatusLog appStatusLog = new AppStatusLog()
                {
                    AppStatusChangedOn = result.Entity.AppliedOn,
                    JobApplicationId = result.Entity.JobApplicationId,
                    AppStatus = 0
                };
                await appDbContext.AppStatusLog.AddAsync(appStatusLog);
                await appDbContext.SaveChangesAsync();

                // commit 1 & 2
                transaction.Commit();
                return result.Entity;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception();
            }
        }

        public async Task<IEnumerable<JobApplication>> GetAllJobApps()
        {
            var jobApps =  appDbContext.JobApplications;
            if (jobApps != null)
                return await jobApps.ToListAsync();
            else
                return new List<JobApplication>();
        }

        public async Task<List<string>> GetAppStatusTypes()
        {
            List<string> appStatusTypes = new List<string>();
            foreach (string appStatusType in Enum.GetNames(typeof(AppStatusType)))
            {
                appStatusTypes.Add(appStatusType);
            }
            return await Task.Run(() => appStatusTypes);
            // return appStatusTypes;
        }

        // ef-core transaction
        public async Task<JobApplication> EditJobApp(JobApplicationEditVM jobApplication)
        {
            // throw new Exception();          

            using var transaction = appDbContext.Database.BeginTransaction();
            try
            {
                var jobApp_ = await appDbContext.JobApplications
                          .Where(x => x.JobApplicationId == jobApplication.JobApplication.JobApplicationId).FirstOrDefaultAsync();
                if (jobApp_ != null)
                {
                    // 1) edit AppStatusLog db table
                    if (jobApp_.AppliedOn.Date != jobApplication.JobApplication.AppliedOn.Date)
                    {
                        var appStatusLogData = await appDbContext.AppStatusLog
                                                .Where(x => x.JobApplicationId == jobApplication.JobApplication.JobApplicationId && x.AppStatus == AppStatusType.Applied).FirstOrDefaultAsync();
                        if (appStatusLogData != null)
                        {
                            appStatusLogData.AppStatusChangedOn = jobApplication.JobApplication.AppliedOn;
                            await appDbContext.SaveChangesAsync();
                        }
                    }


                    // 2) edit JobApplications db table
                    jobApp_.PhoneNumber = jobApplication.JobApplication.PhoneNumber;
                    jobApp_.Province = jobApplication.JobApplication.Province;
                    jobApp_.WebURL = jobApplication.JobApplication.WebURL;
                    jobApp_.FollowUpNotes = jobApplication.JobApplication.FollowUpNotes;
                    jobApp_.ContactPersonName = jobApplication.JobApplication.ContactPersonName;
                    jobApp_.ContactEmail = jobApplication.JobApplication.ContactEmail;
                    jobApp_.CompanyName = jobApplication.JobApplication.CompanyName;
                    jobApp_.City = jobApplication.JobApplication.City;
                    jobApp_.AppStatus = jobApplication.JobApplication.AppStatus;
                    jobApp_.AppliedOn = jobApplication.JobApplication.AppliedOn;
                    jobApp_.AgencyName = jobApplication.JobApplication.AgencyName;
                    await appDbContext.SaveChangesAsync();


                    // throw new Exception();

                    // 3) add into AppStatusLog db table
                    if (jobApplication.AppStatusChanged)
                    {
                        AppStatusLog appStatusLog = new AppStatusLog()
                        {
                            AppStatusChangedOn = jobApplication.AppStatusChangedOn,
                            JobApplicationId = jobApp_.JobApplicationId,
                            AppStatus = jobApplication.JobApplication.AppStatus
                        };
                        await appDbContext.AppStatusLog.AddAsync(appStatusLog);
                        await appDbContext.SaveChangesAsync();
                    }



                    // commit 1 &&/|| 2 &&/|| 3
                    transaction.Commit();

                    return jobApplication.JobApplication;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception();
            }
        }

        public async Task<JobApplication> ViewJobApp(int jobAppId)
        {
            // check for exception
            // throw new Exception();

            JobApplication jobApplication = new JobApplication();

            jobApplication = await appDbContext.JobApplications
                                .Where(x => x.JobApplicationId == jobAppId).FirstOrDefaultAsync();

            return jobApplication;
        }

        public async Task<bool> DeleteJobApp(JobApplication jobApplication)
        {
            try
            {
                // check for exception
                // throw new Exception();

                appDbContext.JobApplications.RemoveRange(appDbContext.JobApplications.Where(x => x.JobApplicationId == jobApplication.JobApplicationId).ToList());
                await appDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<IEnumerable<AppStatusLog>> TrackJobAppStatus(int jobAppId)
        {
            List<AppStatusLog> appStatusLog = new List<AppStatusLog>();

            var appStatusLog_ = appDbContext.AppStatusLog
                            .Where(x => x.JobApplicationId == jobAppId);
            if (appStatusLog_ != null && appStatusLog_.Count() > 0)
            {
                appStatusLog = await appStatusLog_.ToListAsync();
            }
            return appStatusLog;
        }
    }
}
