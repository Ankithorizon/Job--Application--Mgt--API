using EF.Core.Job.Application.Models;
using EF.Core.Job.Application.Context;
using ResumeService.Job.Application.Models;
using ResumeService.Job.Application.Interfaces;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ResumeService.Job.Application.Repositories
{
    public class ResumeCreator : IResumeCreator
    {
        private readonly JobApplicationDBContext appDbContext;

        public ResumeCreator(JobApplicationDBContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public HtmlToPdf GetHtmlToPdfObject()
        {
            HtmlToPdf converter = new HtmlToPdf();

            // header settings
            converter.Options.DisplayHeader = true;
            converter.Header.DisplayOnFirstPage = true;
            converter.Header.DisplayOnOddPages = true;
            converter.Header.DisplayOnEvenPages = true;
            converter.Header.Height = 50;

            // footer settings
            converter.Options.DisplayFooter = true;
            converter.Footer.DisplayOnFirstPage = true;
            converter.Footer.DisplayOnOddPages = true;
            converter.Footer.DisplayOnEvenPages = true;
            converter.Footer.Height = 75;

            // left and right side margin
            converter.Options.MarginLeft = 50;
            converter.Options.MarginRight = 50;

            // set converter options
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.WebPageWidth = 1000;
            converter.Options.WebPageHeight = 1414;

            return converter;
        }

        public string GetPageHeader()
        {
            string pageHeader = @"<html>
                                 <head> 
                                    <style>
                                        body {
                                        }
                                       
                                        .headerDiv{
                                            text-align: center;
                                            vertical-align: middle; 
                                            margin-top: 70px;
                                            margin-bottom: 30px;
                                        }
                                        .flNameDiv{                                               
                                            font-size: 50;   
                                            margin-bottom: 20px;
                                        }
                                        .nameDiv{                                               
                                            font-size: 30;      
                                        }
                                        .anyContent{
                                            padding-left: 10px;
                                            padding-top: 10px;
                                            margin-top: 20px;
                                        }
                                        .sectionHeader{
                                            font-size: 30;   
                                            margin-bottom: 10px;
                                        }
                                        .skillsContent{
                                            font-size: 20; 
                                            padding-top:30px;
                                            padding-left:30px;
                                        }
                                        tr.spaceUnder>td {
                                            padding-bottom: 10px;
                                        }
                                        .skillSpan{
                                            font-size: 25; 
                                        }
                                        table, th, td {
                                          padding-right: 30px;
                                        }
                                        .wexpContent{
                                            font-size: 20; 
                                            padding-top:20px;
                                            padding-left:30px;
                                        }   
                                        .wexpDiv{
                                            padding-bottom:10px;
                                            padding-top:10px;
                                            font-size: 25;
                                        }
                                        .jobResLi{
                                            margin-bottom: 7px;
                                        }
                                        .jobResUi{
                                            padding-bottom: 30px;
                                        }
                                        .durationSpan{
                                            font-size: 25;
                                        }
                                        .educationDiv{
                                            padding-top:5px;
                                            font-size: 25;
                                        }
                                        .educationDurationSpan{
                                            font-size: 25;
                                            padding-top:5px;
                                        }
                                        .educationLi{
                                            padding-bottom:10px;
                                        }
                                        .educationContent{
                                            font-size: 20; 
                                            padding-left:30px;
                                        } 
                                        .educationHeader{
                                            padding-top:-50px;
                                            font-size: 30;
                                        }
                                        .anyEducationContent{
                                            margin-top:-30px;
                                        }
                                    </style>
                                 </head>
                             <body>";
            return pageHeader;
        }
        public string GetPageFooter()
        {
            string pageFooter = @"   </body>
                                   </html>
                                ";
            return pageFooter;

        }
        public string GetPersonalInfoString(PersonalInfo personalInfo)
        {
            string headerString = null;

            headerString = @"
                                <div class='headerDiv'>
                                    <div class='nameDiv'>" +
                                        "<div class='flNameDiv'>" +
                                            personalInfo.FirstName + "&nbsp;" + personalInfo.LastName +
                                        "</div>" +
                                        "Email: " + personalInfo.EmailAddress +
                                        @"<br />" +
                                        "Phone: " + personalInfo.PhoneNumber +
                                    @"</div>  
                                </div>
                                <hr />
                            ";

            return headerString;
        }
            
        public string GetTechnicalSkillsString(List<string> skills)
        {
            StringBuilder skillsString = new StringBuilder();
            skillsString.Append(@"<div class='anyContent'>
                                    <u class='sectionHeader'>Technical Skills: </u>
                                    <br />
                                    <table class='skillsContent'>"
                                );
            int counter = 1;
            foreach (var skill in skills)
            {
                if (counter == 1)
                {
                    counter = 2;
                    skillsString.Append(@"<tr class='spaceUnder'><td class='skillSpan'><b>- </b>" + skill + @"</td>");
                }
                else
                {
                    counter = 1;
                    skillsString.Append(@"<td class='skillSpan'><b>- </b>" + skill + @"</td></tr>");
                }
            }
            skillsString.Append(@"</table></div>");
            return skillsString.ToString();
        }

        public string GetWorkExperienceString(List<WorkExperience> workExperiences)
        {
            StringBuilder woExpString = new StringBuilder();

            woExpString.Append(@"<div class='anyContent'>
                                    <u class='sectionHeader'>Work Experience: </u>
                                    <br />
                                    <div class='wexpContent'>
                                        <div class='wexpDiv'>"
                                );

            foreach (var workExperience in workExperiences)
            {
                woExpString.Append(@"<b>Client: " + workExperience.EmployerName + "  -  " + workExperience.City + ", " + workExperience.Province + "</b></div>");
                woExpString.Append(@"<div class='durationSpan'>");
                woExpString.Append(@"Duration: " + workExperience.StartDate + " - " + workExperience.EndDate + "</div>");
                woExpString.Append(@"<br /><div class='wexpDiv'>");
                woExpString.Append(@"Job Responsibilities: <ul class='jobResUi'>");

                foreach (var jobRes in workExperience.JobDetails)
                {
                    woExpString.Append("<li class='jobResLi'>" + jobRes + "</li>");
                }
                woExpString.Append("</ul>");
            }


            woExpString.Append("</div></div></div>");

            return woExpString.ToString();
        }

        public string GetEducationString(List<Education> educations)
        {
            StringBuilder educationString = new StringBuilder();

            educationString.Append(@"<div class='anyEducationContent'>
                                    <u class='educationHeader'>Education: </u>
                                    <br />
                                    <div class='educationContent'><ul>"
                                );

            foreach (var education in educations)
            {
                educationString.Append(@"<li class='educationLi'>");
                educationString.Append(@"<div class='educationDiv'><b>" + education.DegreeName + "</b></div>");
                if (education.Major != null && education.Major != "")
                {
                    educationString.Append(@"<div class='educationDiv'><b>Major : " + education.Major + "</b></div>");
                }
                educationString.Append(@"<div class='educationDurationSpan'>");
                educationString.Append(@"(" + education.StartDate + " - " + education.EndDate + ")</div>");
                educationString.Append(@"<div class='educationDurationSpan'>");
                educationString.Append(@"" + education.UniversityName + ", " + education.Country + "</div></li>");
            }

            educationString.Append("</ul></div></div>");

            return educationString.ToString();
        }

        public bool AddUserDataWhenResumeDownloaded(UserResumeCreate userData)
        {
            try
            {
                appDbContext.UserResumeCreate.Add(userData);
                appDbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public IEnumerable<UserResumeCreate> GetUserResumeDownloadData()
        {
            var userDatas = appDbContext.UserResumeCreate;
            if (userDatas != null)
                return userDatas;
            else
                return new List<UserResumeCreate>();
        }

        public bool AddUserDataWhenResumeEmailed(UserResumeEmail userData)
        {
            try
            {
                appDbContext.UserResumeEmail.Add(userData);
                appDbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public IEnumerable<UserResumeEmail> GetUserResumeEmailData()
        {
            var userDatas = appDbContext.UserResumeEmail;
            if (userDatas != null)
                return userDatas;
            else
                return new List<UserResumeEmail>();
        }

    }
}
