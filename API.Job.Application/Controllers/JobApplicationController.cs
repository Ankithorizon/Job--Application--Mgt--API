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
    public class JobApplicationController : ControllerBase
    {
        // file upload location settings from appsettings.json
        private readonly IConfiguration _configuration;

        private APIResponse _response;
        private readonly IJobApplicationRepository _jobAppRepo;

        public JobApplicationController(IConfiguration configuration, IJobApplicationRepository jobAppRepo)
        {
            _jobAppRepo = jobAppRepo;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("getAllJobApps")]
        public IActionResult GetAllJobApps()
        {
            try
            {
                var allJobApps = _jobAppRepo.GetAllJobApps();
                return Ok(allJobApps);
            }
            catch(Exception ex)
            {
                return BadRequest();
            }         
        }

        [HttpGet]
        [Route("getAppStatusTypes")]
        public IActionResult GetAppStatusTypes()
        {
            try
            {
                var appStatusTypes = _jobAppRepo.GetAppStatusTypes();
                return Ok(appStatusTypes);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("viewJobApp/{jobAppId}")]
        public IActionResult ViewJobApp(int jobAppId)
        {
            try
            {
                var jobApp = _jobAppRepo.ViewJobApp(jobAppId);
                return Ok(jobApp);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Service Not Available!");
            }
        }

        [HttpPost]
        [Route("addJobApp")]
        public IActionResult AddJobApp(JobApplication jobAppData)
        {
            _response = new APIResponse();
            try
            {
                // jobAppData = null;
                if (jobAppData == null)
                {
                    _response.ResponseCode = -1;
                    _response.ResponseMessage = "Job-Application is Null!";
                    return BadRequest(_response);
                }

                // throw new Exception();

                // check for ModelState
                // ModelState.AddModelError("contactPersonName", "Contact Person Name is Required!");
                // ModelState.AddModelError("contactEmail", "Contact Email is Required!");

                if (ModelState.IsValid)
                {
                    _jobAppRepo.AddJobApp(jobAppData);
                    _response.ResponseCode = 0;
                    _response.ResponseMessage = "Job Applied Successfully !";
                    return Ok(_response);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server Error !");
            }
        }


        [HttpPost]
        [Route("editJobApp")]
        public IActionResult EditJobApp(JobApplicationEditVM jobAppData)
        {
            _response = new APIResponse();
            try
            {
                // jobAppData = null;
                if (jobAppData == null)
                {
                    _response.ResponseCode = -1;
                    _response.ResponseMessage = "Job-Application is Null!";
                    return BadRequest(_response);
                }

                // throw new Exception();

                // check for ModelState
                // ModelState.AddModelError("contactPersonName", "Contact Person Name is Required!");
                // ModelState.AddModelError("contactEmail", "Contact Email is Required!");

                if (ModelState.IsValid)
                {
                    // check for appStatus==Closed
                    // user can't edit this job-app
                    if (_jobAppRepo.JobAppClosed(jobAppData.JobApplication.JobApplicationId))
                    {
                        _response.ResponseCode = -1;
                        _response.ResponseMessage = "This Job-Application is already CLOSED!";
                        return BadRequest(_response);
                    }


                    if (_jobAppRepo.EditJobApp(jobAppData) != null)
                    {
                        _response.ResponseCode = 0;
                        _response.ResponseMessage = "Job Edited Successfully !";
                        return Ok(_response);
                    }
                    else
                    {
                        _response.ResponseCode = -1;
                        _response.ResponseMessage = "Data Not Found on Server!";
                        return BadRequest(_response);                        
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server Error !");
            }
        }


        [HttpPost]
        [Route("deleteJobApp")]
        public IActionResult DeleteJobApp(JobApplication jobAppData)
        {
            _response = new APIResponse();
            try
            {
                // jobAppData = null;
                if (jobAppData == null)
                {
                    _response.ResponseCode = -1;
                    _response.ResponseMessage = "Job-Application is Null!";
                    return BadRequest(_response);
                }

                // throw new Exception();            

                if (_jobAppRepo.DeleteJobApp(jobAppData))
                {
                    _response.ResponseCode = 0;
                    _response.ResponseMessage = "Job Deleted Successfully";
                    return Ok(_response);
                }
                else
                {
                    _response.ResponseCode = -1;
                    _response.ResponseMessage = "Delete Job-Application Fail!";
                    return BadRequest(_response);
                }                    
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server Error !");
            }
        }


        [HttpGet]
        [Route("trackJobApp/{jobAppId}")]
        public IActionResult TrackJobApps(int jobAppId)
        {
            try
            {
                // throw new Exception();

                var appStatusLog = _jobAppRepo.TrackJobAppStatus(jobAppId);
                return Ok(appStatusLog);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Server Error !");
            }
        }
    }
}
