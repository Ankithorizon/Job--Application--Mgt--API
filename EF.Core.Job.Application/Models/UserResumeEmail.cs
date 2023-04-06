using System;
using System.Collections.Generic;
using System.Text;

namespace EF.Core.Job.Application.Models
{
    public class UserResumeEmail
    {
        public int UserResumeEmailId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserEmail { get; set; }
        public DateTime ResumeEmailedAt { get; set; }
    }
}
