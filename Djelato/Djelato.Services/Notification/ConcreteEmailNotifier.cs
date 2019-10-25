using Djelato.Common.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Djelato.Services.Notification
{
    public class ConcreteEmailNotifier : INotifier
    {
        private readonly EmailSettings _settings;
        public ConcreteEmailNotifier(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendKey(string email, int key)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_settings.Sender, _settings.Sender));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = "Djelato Confirm profile";

            var builder = new BodyBuilder();

            builder.HtmlBody = string.Format(@"<p>Nice to meet you</p>
<p>We glad to see you in Djelato company website. We send you the link to confirm the password</p>
please copy this <strong>{0}</strong> code to form to confirm an account or click the link <br/>
<a href=""http://localhost:4200/home/{0}"">Click here</a>
<p>Thank you for your time, Djelato administration</p>
", key);


            emailMessage.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_settings.MailServer, _settings.MailPort, false);
                await client.AuthenticateAsync(_settings.Sender, _settings.Password);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }

        }
    }
}
