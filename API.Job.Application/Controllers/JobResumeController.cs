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

namespace API.Job.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobResumeController : ControllerBase
    {
        // file upload location settings from appsettings.json
        private readonly IConfiguration _configuration;

        private APIResponse _response;
        private readonly IJobResumeRepository _jobResumeRepo;

        // check for file type
        // pdf
        private string[] permittedExtensions = { ".pdf" };

        public JobResumeController(IConfiguration configuration, IJobResumeRepository jobResumeRepo)
        {
            _jobResumeRepo = jobResumeRepo;
            _configuration = configuration;
        }

        // file-upload
        [HttpPost, DisableRequestSizeLimit]
        [Route("uploadResume")]
        public IActionResult UploadResume([FromForm] ResumeUpload resumeUpload)
        {
            _response = new APIResponse();
            try
            {
                // check for exception
                // throw new Exception();

                // resumeUpload = null;
                if (resumeUpload == null)
                {
                    _response.ResponseCode = -1;
                    _response.ResponseMessage = "Object Null Error!";
                    return BadRequest(_response);
                }

                // resumeUpload.JobApplicationId = null;
                if (resumeUpload.JobApplicationId == null)
                {
                    _response.ResponseCode = -1;
                    _response.ResponseMessage = "Job-Application Object Null Error!";
                    return BadRequest(_response);
                }
                else
                {
                    // check for appStatus==Closed
                    // user can't edit this job-app
                    if (_jobResumeRepo.JobAppClosed(Convert.ToInt32(resumeUpload.JobApplicationId)))
                    {
                        _response.ResponseCode = -1;
                        _response.ResponseMessage = "This Job-Application is already CLOSED!";
                        return BadRequest(_response);
                    }
                }

                // resumeUpload.JobApplicationId = "Bad Job-Application Object";
                int jobApplicationId = Int32.Parse(resumeUpload.JobApplicationId);
                // var file = Request.Form.Files[0];
                var file = resumeUpload.ResumeFile;

                // check for file type
                // .pdf
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
                {
                    _response.ResponseCode = -1;
                    _response.ResponseMessage = "Invalid File Type!,,, Only .PDF File Is Allowed To Upload!";
                    return BadRequest(_response);
                }

                string resumeStoragePath = _configuration.GetSection("ResumeUploadLocation").GetSection("Path").Value;

                // unique random number to edit file name
                var guid = Guid.NewGuid();
                var bytes = guid.ToByteArray();
                var rawValue = BitConverter.ToInt64(bytes, 0);
                var inRangeValue = Math.Abs(rawValue) % DateTime.MaxValue.Ticks;

                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), resumeStoragePath);

                // check for 500
                // file = null;

                if (file.Length > 0)
                {
                    var fileName = inRangeValue + "_" + ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);

                    // file-system store
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    // db store
                    JobResume jobResume = new JobResume()
                    {
                        FileName = fileName,
                        FilePath = pathToSave,
                        JobApplicationId = jobApplicationId
                    };
                    if (_jobResumeRepo.StoreResumeFile(jobResume))
                    {
                        _response.ResponseCode = 0;
                        _response.ResponseMessage = "Resume Upload Success !";
                        return Ok(_response);
                    }
                    else
                    {
                        _response.ResponseCode = -1;
                        _response.ResponseMessage = "Database Error !";
                        return BadRequest(_response);
                    }
                }
                else
                {
                    return BadRequest("Nothing To Upload !");
                }
            }
            catch (FormatException)
            {
                _response.ResponseCode = -1;
                _response.ResponseMessage = "Invalid Job-Application Object !";
                return BadRequest(_response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server Error !");
            }
        }

        // file-download
        [HttpGet, DisableRequestSizeLimit]
        [Route("downloadResume/{jobApplicationId}")]
        public async Task<IActionResult> DownloadResume(int jobApplicationId)
        {
            try
            {
                // check for exception
                // throw new Exception();

                // check if file exist @ database
                string fileName = _jobResumeRepo.GetResumeFile(jobApplicationId);
                if (fileName == null)
                {
                    return BadRequest();
                }

                string resumeStoragePath = _configuration.GetSection("ResumeUploadLocation").GetSection("Path").Value;
                var currentDirectory = System.IO.Directory.GetCurrentDirectory();
                currentDirectory = currentDirectory + "\\" + resumeStoragePath;
                var file = Path.Combine(currentDirectory, fileName);

                // check if file exists @ file-system
                if (System.IO.File.Exists(file))
                {
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(file, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }

                    memory.Position = 0;
                    return File(memory, GetMimeType(file), fileName);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, "Server Error!");
            }
        }
        private string GetMimeType(string file)
        {
            string extension = Path.GetExtension(file).ToLowerInvariant();
            switch (extension)
            {
                case ".txt": return "text/plain";
                case ".pdf": return "application/pdf";
                case ".doc": return "application/vnd.ms-word";
                case ".docx": return "application/vnd.ms-word";
                case ".xls": return "application/vnd.ms-excel";
                case ".png": return "image/png";
                case ".jpg": return "image/jpeg";
                case ".jpeg": return "image/jpeg";
                case ".gif": return "image/gif";
                case ".csv": return "text/csv";
                default: return "";
            }
        }



    }
}
