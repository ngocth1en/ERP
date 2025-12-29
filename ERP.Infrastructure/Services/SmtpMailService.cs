using ERP.Application.Auth.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Services
{
    public class SmtpMailService : IMailService
    {
        private readonly SmtpClient _client;
        private readonly string _from;

        public SmtpMailService(IConfiguration config)
        {
            _from = config["Mail:From"];

            _client = new SmtpClient(config["Mail:Host"])
            {
                Port = int.Parse(config["Mail:Port"]),
                Credentials = new NetworkCredential(
                    config["Mail:Username"],
                    config["Mail:Password"]),
                EnableSsl = true
            };
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var mail = new MailMessage(_from, to, subject, body);
            await _client.SendMailAsync(mail);
        }
    }

}
