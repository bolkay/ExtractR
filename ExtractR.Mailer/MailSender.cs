using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;

namespace ExtractR.Mailer
{
    public class MailSender
    {
        public MailSender()
        {

        }
        public async Task<bool> TrySendSimpleMail(string message, string subject, string from, string to, bool isFeatureRequest)
        {   
            string host = Environment.GetEnvironmentVariable("smtp_host");
            int port = int.Parse(Environment.GetEnvironmentVariable("smtp_port"));
            string username = Environment.GetEnvironmentVariable("smtp_username");
            string password = Environment.GetEnvironmentVariable("smtp_password");

            if (!InputsAreValid(message, subject, from, to, username, password, host, port.ToString()))
                return false;

            try
            {
                if (isFeatureRequest)
                    message = $"FEATURE REQUEST : {message}";
                else
                    message = $"ISSUE REPORT : {message}";

                MimeMessage mailMessage = new MimeMessage();
                mailMessage.Sender = MailboxAddress.Parse(from);
                mailMessage.To.Add(MailboxAddress.Parse(to));
                mailMessage.Subject = subject;

                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = message;

                mailMessage.Body = bodyBuilder.ToMessageBody();

                using (MailKit.Net.Smtp.SmtpClient smtpClient = new MailKit.Net.Smtp.SmtpClient())
                {
                    await smtpClient.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.Auto);

                    await smtpClient.AuthenticateAsync(new NetworkCredential(username, password));

                    await smtpClient.SendAsync(mailMessage);

                    await smtpClient.DisconnectAsync(true);

                    return true;
                }
            }
            catch(Exception ex) { return false; }
        }
        private bool InputsAreValid(params string[] parameters)
        {
            return parameters.Any(x => string.IsNullOrEmpty(x));
        }
    }
}
