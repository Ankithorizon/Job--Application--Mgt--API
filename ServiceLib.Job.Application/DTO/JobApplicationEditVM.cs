using EF.Core.Job.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ServiceLib.Job.Application.DTO
{
    public class JobApplicationEditVM
    {
        public JobApplication JobApplication { get; set; }
        public bool AppStatusChanged { get; set; }
        public DateTime AppStatusChangedOn { get; set; }
    }
}
