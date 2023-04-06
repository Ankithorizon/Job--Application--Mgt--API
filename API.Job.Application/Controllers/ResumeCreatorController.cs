using EF.Core.Job.Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceLib.Job.Application.DTO;
using ServiceLib.Job.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Http.Headers;
using System.Web;
using ResumeService.Job.Application.Interfaces;
using ResumeService.Job.Application.Models;
using SelectPdf;
using System.Text;
using EmailService.Job.Application.Interfaces;
using EmailService.Job.Application.Models;

namespace API.Job.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResumeCreatorController : ControllerBase
    {
        private APIResponse _response;

        private readonly IResumeCreator _resumeCreator;
        private readonly IEmailSender _emailSender;


        public ResumeCreatorController(IResumeCreator resumeCreator, IEmailSender emailSender)
        {
            _resumeCreator = resumeCreator;
            _emailSender = emailSender;
        }


        // create pdf resume as byte[] and display @ browser
        [HttpPost]
        [Route("createAndDownloadResume")]
        public IActionResult CreateAndDownloadResume(MyResume myResume)
        {
            _response = new APIResponse();

            try
            {

                // throw new Exception();

                // instantiate a html to pdf converter object
                HtmlToPdf converter = _resumeCreator.GetHtmlToPdfObject();

                // prepare data
                // Personal Info
                PersonalInfo personalInfo = new PersonalInfo();
                personalInfo = myResume.PersonalInfo;
                if (personalInfo == null)
                {
                    _response.ResponseCode = -1;
                    _response.ResponseMessage = "Personal-Info Null!";
                    return BadRequest(_response);
                }

                // Technical Skills List<string>
                List<string> skills = new List<string>();
                skills = myResume.Skills;
                if (skills == null)
                {
                    _response.ResponseCode = -1;
                    _response.ResponseMessage = "Technical-Skills Null!";
                    return BadRequest(_response);
                }

                // Work Experience
                List<WorkExperience> workExps = new List<WorkExperience>();
                workExps = myResume.WorkExperience;
                if (workExps == null)
                {
                    _response.ResponseCode = -1;
                    _response.ResponseMessage = "Work-Experience Null!";
                    return BadRequest(_response);
                }

                // Education              
                List<Education> educations = new List<Education>();
                educations = myResume.Education;
                if (educations == null)
                {
                    _response.ResponseCode = -1;
                    _response.ResponseMessage = "Education Null!";
                    return BadRequest(_response);
                }

                var content = _resumeCreator.GetPageHeader() +
                                _resumeCreator.GetPersonalInfoString(personalInfo) +
                                _resumeCreator.GetTechnicalSkillsString(skills) +
                                _resumeCreator.GetWorkExperienceString(workExps) +
                                _resumeCreator.GetEducationString(educations) +
                                _resumeCreator.GetPageFooter();

                // create pdf as byte[] and display @ browser
                var pdf = converter.ConvertHtmlString(content);
                var pdfBytes = pdf.Save();


                // UserResumeCreate db-table
                // process to add client's ip address and datetime
                // and PersonalInfo>FirstName and LastName @ db
                var hostName = System.Net.Dns.GetHostName();
                var ips = System.Net.Dns.GetHostAddresses(hostName);
                StringBuilder myIpAddress = new StringBuilder();
                foreach (var ip in ips)
                {
                    myIpAddress.Append(ip.ToString() + ",");
                }
                UserResumeCreate userData = new UserResumeCreate()
                {
                    FirstName = personalInfo.FirstName,
                    LastName = personalInfo.LastName,
                    ResumeCreatedAt = DateTime.Now,
                    UserIPAddress = myIpAddress.ToString().Substring(0, (myIpAddress.ToString().Length))
                };
                if (_resumeCreator.AddUserDataWhenResumeDownloaded(userData))
                {
                    return File(pdfBytes, "application/pdf");
                    // return File(pdfBytes, "application/pdf", "your_resume.pdf");
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Memory File Creation Error!");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Server Error!");
            }
        }


        // create pdf resume as byte[] 
        // and attach it as email attachment,
        // but do not store .pdf file on server
        [HttpPost]
        [Route("createAndEmailResume")]
        public async Task<ActionResult> CreateAndEmailResume(MyResume myResume)
        {
            _response = new APIResponse();
            try
            {
                // throw new Exception();

                // instantiate a html to pdf converter object
                HtmlToPdf converter = _resumeCreator.GetHtmlToPdfObject();

                // prepare data
                // Personal Info
                PersonalInfo personalInfo = new PersonalInfo();
                personalInfo = myResume.PersonalInfo;
                if (personalInfo == null)
                {
                    _response.ResponseCode = -1;
                    _response.ResponseMessage = "Personal-Info Null!";
                    return BadRequest(_response);
                }

                // Technical Skills List<string>
                List<string> skills = new List<string>();
                skills = myResume.Skills;
                if (skills == null)
                {
                    _response.ResponseCode = -1;
                    _response.ResponseMessage = "Technical-Skills Null!";
                    return BadRequest(_response);
                }

                // Work Experience
                List<WorkExperience> workExps = new List<WorkExperience>();
                workExps = myResume.WorkExperience;
                if (workExps == null)
                {
                    _response.ResponseCode = -1;
                    _response.ResponseMessage = "Work-Experience Null!";
                    return BadRequest(_response);
                }

                // Education
                List<Education> educations = new List<Education>();
                educations = myResume.Education;
                if (educations == null)
                {
                    _response.ResponseCode = -1;
                    _response.ResponseMessage = "Education Null!";
                    return BadRequest(_response);
                }


                var content = _resumeCreator.GetPageHeader() +
                                _resumeCreator.GetPersonalInfoString(personalInfo) +
                                _resumeCreator.GetTechnicalSkillsString(skills) +
                                _resumeCreator.GetWorkExperienceString(workExps) +
                                _resumeCreator.GetEducationString(educations) +
                                _resumeCreator.GetPageFooter();

                var pdf = converter.ConvertHtmlString(content);
                var pdfBytes = pdf.Save();

                // UserResumeEmail db-table
                // process to add client's email address and datetime
                // and PersonalInfo>FirstName and LastName @ db
                // myResume.EmailMyResumeTo = "ankitjpatel2007@hotmail.com";
                myResume.EmailMyResumeTo = personalInfo.EmailAddress;
                UserResumeEmail userData = new UserResumeEmail()
                {
                    FirstName = personalInfo.FirstName,
                    LastName = personalInfo.LastName,
                    ResumeEmailedAt = DateTime.Now,
                    UserEmail = myResume.EmailMyResumeTo
                };
                if (_resumeCreator.AddUserDataWhenResumeEmailed(userData))
                {
                    // convert byte[] to memory-stream
                    MemoryStream stream = new MemoryStream(pdfBytes);
                    // create .pdf and attach it as email attachment, but do not store .pdf file on server
                    var message = new Message(new string[] { myResume.EmailMyResumeTo }, "Please check your Resume in attachment!", "[Job-Apps-MGT] #Resume-Creator and #Email-Attachment Service", null, stream, "pdf", "myResume.pdf");
                    await _emailSender.SendEmailAsync(message);

                    return Ok("Resume sent in your Email-Attachment! Please check your Email!");
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Sending Email Error!");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Server Error!");
            }
        }


        [HttpGet]
        [Route("getUserResumeDownloadData")]
        public IActionResult GetUserResumeDownloadData()
        {
            try
            {
                var userDatas = _resumeCreator.GetUserResumeDownloadData();
                return Ok(userDatas);
            }
            catch(Exception ex)
            {
                return BadRequest();
            }          
        }

        [HttpGet]
        [Route("getUserResumeEmailData")]
        public IActionResult GetUserResumeEmailData()
        {
            try
            {
                var userDatas = _resumeCreator.GetUserResumeEmailData();
                return Ok(userDatas);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

    }
}
