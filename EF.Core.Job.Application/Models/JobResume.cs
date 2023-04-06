using System;
using System.Collections.Generic;
using System.Text;

namespace EF.Core.Job.Application.Models
{
    public class JobResume
    {
        public int JobResumeId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public int JobApplicationId { get; set; }
        public JobApplication JobApplication { get; set; }
    }
}
