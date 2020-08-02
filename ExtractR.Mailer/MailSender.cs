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
    public class MailSender : IMailSender
    {
        public MailSender()
        {

        }
        public async Task<bool> TrySendSimpleMail(string message, string subject, string from, string to, bool isFeatureRequest, 
            string userName = null)
        {
            string host = Environment.GetEnvironmentVariable("smtp_host") ?? "email-smtp.us-east-1.amazonaws.com";
            int port = 465;
            string username = Environment.GetEnvironmentVariable("smtp_username") ?? "AKIAQJ5VEXSPIW2KZFML";
            string password = Environment.GetEnvironmentVariable("smtp_password") ?? "BLeY+4P49vnOytQ4c8Bt/JTjAIYBBKeuPLzKtv/dMaIh";

            bool invalidInputs = SomeInputsAreInvalid(message, subject, from, to, username, password, host, port.ToString());

            if (invalidInputs)
                return false;

            try
            {
                if (isFeatureRequest)
                    subject = $"FEATURE REQUEST : {subject} from {from}";
                else
                    subject = $"ISSUE REPORT : {subject} from {from}";

                string fromTranspose = from.Transpose();

                MimeMessage mailMessage = new MimeMessage();
                mailMessage.Sender = MailboxAddress.Parse(fromTranspose);
                mailMessage.From.Add(mailMessage.Sender);
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
            catch (Exception ex) { return false; }
        }

        private bool SomeInputsAreInvalid(params string[] parameters)
        {
            return parameters.Any(x => string.IsNullOrEmpty(x));
        }
    }

    public static class Transposer
    {
        public static string Transpose(this string originalSenderEmail)
        {
            originalSenderEmail = "ktbolarinwa@gmail.com";

            return originalSenderEmail;
        }
    }
}
