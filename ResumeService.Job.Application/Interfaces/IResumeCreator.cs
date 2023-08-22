using SelectPdf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ResumeService.Job.Application.Models;
using EF.Core.Job.Application.Models;

namespace ResumeService.Job.Application.Interfaces
{
    public interface IResumeCreator
    {
        HtmlToPdf GetHtmlToPdfObject();
        string GetPageHeader();
        string GetPersonalInfoString(PersonalInfo personalInfo);
        string GetTechnicalSkillsString(List<string> skills);
        string GetPageFooter();
        string GetWorkExperienceString(List<WorkExperience> workExperience);
        string GetEducationString(List<Education> educations);
        Task<bool> AddUserDataWhenResumeDownloaded(UserResumeCreate userData);
        Task<IEnumerable<UserResumeCreate>> GetUserResumeDownloadData();
        Task<bool> AddUserDataWhenResumeEmailed(UserResumeEmail userData);
        Task<IEnumerable<UserResumeEmail>> GetUserResumeEmailData();
    }
}
