using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmailService.Job.Application.Models;
using EmailService.Job.Application.Interfaces;

namespace EmailService.Job.Application.Repositories
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;

        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);

            Send(emailMessage);
        }

        public async Task SendEmailAsync(Message message)
        {
            if (message.Attachments != null && message.Attachments.Any())
            {
                // email with file-attachment
                // file either coming from upload-action or stored previously on server file-system
                var mailMessage = CreateEmailMessage(message);
                await SendAsync(mailMessage);
            }
            else
            {
                // email with memory-stream to byte[] as email-attachment
                // no file storage @ server side
                var mailMessage = CreateEmailMessage_(message);
                await SendAsync(mailMessage);
            }
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = string.Format("<h2 style='color:red;'>{0}</h2>", message.Content) };

            if (message.Attachments != null && message.Attachments.Any())
            {
                byte[] fileBytes;
                foreach (var attachment in message.Attachments)
                {
                    using (var ms = new MemoryStream())
                    {
                        attachment.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }

                    bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
                }
            }

            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

                    client.Send(mailMessage);
                }
                catch
                {
                    //log an error message or throw an exception, or both.
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }

        private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);

                    await client.SendAsync(mailMessage);
                }
                catch
                {
                    //log an error message or throw an exception, or both.
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }

        // email with attachment file,,, .pdf, .csv,,,
        // do not store file on server
        // just attach as byte[],,,  message.DataAsMemoryStream.ToArray()
        private MimeMessage CreateEmailMessage_(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = string.Format("<h2 style='color:red;'>{0}</h2>", message.Content) };

            System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType();

            // .csv
            if (message.FileType == "csv")
            {
                ct = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Text.Plain);
                System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(message.DataAsMemoryStream, ct);
                attach.ContentDisposition.FileName = message.FileName;
                bodyBuilder.Attachments.Add(message.FileName, message.DataAsMemoryStream.ToArray(), ContentType.Parse("text/csv"));
            }
            else if (message.FileType == "pdf")
            {
                ct = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);
                System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(message.DataAsMemoryStream, ct);
                attach.ContentDisposition.FileName = message.FileName;
                bodyBuilder.Attachments.Add(message.FileName, message.DataAsMemoryStream.ToArray(), ContentType.Parse("application/pdf"));
            }

            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }


        // convert string into memory-stream
        public MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }
    }
}
