using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using EmailService.Job.Application.Models;


namespace EmailService.Job.Application.Interfaces
{
    public interface IEmailSender
    {
        void SendEmail(Message message);
        Task SendEmailAsync(Message message);
        MemoryStream GenerateStreamFromString(string value);
    }
}
