using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeService.Job.Application.Models
{
    public class Education
    {
        public string UniversityName { get; set; }
        public string Country { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string DegreeName { get; set; }
        public string Major { get; set; }
    }
}
