using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace TMDemo.Models
{
    public class EmailSender : IEmailSender
    {
        //private readonly IConfiguration _configuration;

        //public EmailSender(IConfiguration )
        //{
        //    this.smtpServer = smtpServer;
        //    this.smtpPort = smtpPort;
        //}

        //public EmailSender(string smtpHost, int smtpPort, string fromEmail, string fromPassword)
        //{
        //    _smtpClient = new SmtpClient(smtpHost, smtpPort)
        //    {
        //        Credentials = new NetworkCredential(fromEmail, fromPassword),
        //        EnableSsl = true
        //    };
        //    _fromEmail = fromEmail;
        //}

        //public Task SendEmailAsync(string to, string subject, string message)
        //{
        //    var mailMessage = new MailMessage(_fromEmail, to, subject, message);
        //    return _smtpClient.SendMailAsync(mailMessage);
        //}
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var smtpClient = new SmtpClient(_configuration["Smtp:Host"])
            {
                Port = int.Parse(_configuration["Smtp:Port"]),
                Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Smtp:From"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            return smtpClient.SendMailAsync(mailMessage);
        }
    }
}
