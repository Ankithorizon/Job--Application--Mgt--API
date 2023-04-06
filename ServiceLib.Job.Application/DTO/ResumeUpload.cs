using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ServiceLib.Job.Application.DTO
{
    public class ResumeUpload
    {
        public string JobApplicationId { get; set; }
        public IFormFile ResumeFile { get; set; }
    }
}
