using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeService.Job.Application.Models
{
    public class WorkExperience
    {
        public string EmployerName { get; set; }
        public string StartDate { get; set; } // month, year 
        public string EndDate { get; set; } // month, year / till-date
        public string City { get; set; }
        public string Province { get; set; }
        public List<string> JobDetails { get; set; }

    }
}
