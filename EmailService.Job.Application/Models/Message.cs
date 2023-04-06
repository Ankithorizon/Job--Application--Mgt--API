using Microsoft.AspNetCore.Http;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EmailService.Job.Application.Models
{
    public class Message
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public IFormFileCollection Attachments { get; set; }

        public Message(IEnumerable<string> to, string subject, string content, IFormFileCollection attachments, MemoryStream dataAsMemoryStream, string fileType, string fileName)
        {
            To = new List<MailboxAddress>();

            To.AddRange(to.Select(x => new MailboxAddress(x)));
            Subject = subject;
            Content = content;
            Attachments = attachments;
            DataAsMemoryStream = dataAsMemoryStream;
            FileType = fileType;
            FileName = fileName;
        }

        // memory-stream to byte[] as file-attachment
        public MemoryStream DataAsMemoryStream { get; set; }

        // file type,,, .pdf, .csv,,,
        public string FileType { get; set; }

        // file name
        public string FileName { get; set; }
    }
}
