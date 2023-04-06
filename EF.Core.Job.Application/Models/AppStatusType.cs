using System;
using System.Collections.Generic;
using System.Text;

namespace EF.Core.Job.Application.Models
{
    public enum AppStatusType
    {
        Applied,
        Follow_Up,
        Client_Response,
        Interview_Setup,
        Interview_Done,
        Final_Result,
        Closed
    }
}
