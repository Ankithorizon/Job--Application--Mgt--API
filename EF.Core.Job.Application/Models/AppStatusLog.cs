using System;
using System.Collections.Generic;
using System.Text;

namespace EF.Core.Job.Application.Models
{
    public class AppStatusLog
    {
        public int AppStatusLogId { get; set; }
        public AppStatusType AppStatus { get; set; }
        public DateTime AppStatusChangedOn { get; set; }
        public int JobApplicationId { get; set; }
        public JobApplication JobApplication { get; set; }
    }
}
