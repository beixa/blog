using Blog.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _client;
        private readonly IOptions<SmtpSettings> _options;

        public EmailService(IOptions<SmtpSettings> options)
        {
            _options = options;
            _client = new SmtpClient(options.Value.Server)
            { 
                Credentials = new NetworkCredential(options.Value.Username, options.Value.Password)
            };
            
        }
        public Task SendEmail(string email, string subject, string message)
        {
            var mailMessage = new MailMessage(_options.Value.From, email, subject, message);
            return _client.SendMailAsync(mailMessage);
        }
    }
}
