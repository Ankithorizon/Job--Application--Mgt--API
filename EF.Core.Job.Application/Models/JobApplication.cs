using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EF.Core.Job.Application.Models
{
    public class JobApplication
    {
        public int JobApplicationId { get; set; }
        public string CompanyName { get; set; }
        public string AgencyName { get; set; }
        public string WebURL { get; set; }

        [Required(ErrorMessage = "Contact Person Name is Required!")]
        public string ContactPersonName { get; set; }

        [Required(ErrorMessage = "Contact Email is Required!")]
        public string ContactEmail { get; set; }
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "City is Required!")]
        public string City { get; set; }

        [Required(ErrorMessage = "Province is Required!")]
        public string Province { get; set; }

        [Required(ErrorMessage = "Applied On is Required!")]
        public DateTime AppliedOn { get; set; }
        public AppStatusType AppStatus { get; set; }
        public string FollowUpNotes { get; set; }

        public JobResume JobResume { get; set; }
        public ICollection<AppStatusLog> AppStatusLog { get; set; }
    }
}
