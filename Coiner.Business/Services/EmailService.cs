using System;
using System.Collections.Generic;
using System.Text;
using Coiner.Business.Models;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.IO;
using Coiner.Business.Heplers;

namespace Coiner.Business.Services
{
    public class EmailService
    {
        private IConfiguration config;
        private SmtpClient smtpClient;

        public bool EnableNotficationEmailsCheck { get; set; }

        public EmailService()
        {
            config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                               .AddJsonFile("appsettings.json", true, true)
                                               .Build();

            EnableNotficationEmailsCheck = Convert.ToBoolean(config["EnableNotficationEmails"]);

            var smtpConfig = config.GetSection("SmtpClient");

            smtpClient = new SmtpClient
            {
                UseDefaultCredentials = Convert.ToBoolean(smtpConfig["DefaultCredentials"]),
                EnableSsl = Convert.ToBoolean(smtpConfig["EnableSsl"]),
                Host = smtpConfig["Host"],
                Port = Convert.ToInt32(smtpConfig["Port"]),
                Credentials = new System.Net.NetworkCredential(smtpConfig["Username"], smtpConfig["Password"])
            };
        }

        public virtual void SendUserCreationEmail(User user)
        {   //encode the userID in base64
            var encodedId = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(user.Id.ToString()));
            var content = "";
            if (user.Provider == Models.Enums.ProvidersEnum.Coiner) {
                content = EmailTemplate.EmailContent(UserCreationEmailTemplate.Path)
                                           .Replace(EmailTemplate.UserName, user.FirstName)
                                           .Replace(EmailTemplate.ActivationToken, user.ActivationToken)
                                           .Replace(EmailTemplate.UserId, encodedId)
                                           .Replace(EmailTemplate.BaseUrl, Constants.BaseUrl);
            }
            else
            {
                 content = EmailTemplate.EmailContent(UserCreationGoogleEmailTemplate.Path)
                                           .Replace(EmailTemplate.UserName, user.FirstName)
                                           .Replace(EmailTemplate.BaseUrl, Constants.BaseUrl);
            }

            SendEmail(user.Email, UserCreationEmailTemplate.Subject, content);
        }

        public void SendEmail(string UserEmail, string subject, string body)
        {
            if (!EnableNotficationEmailsCheck)
            {
                return;
            }

            var mail = MessageInitialize(UserEmail, subject);
            mail.Body = body;
            smtpClient.Send(mail);
        }

        private MailMessage MessageInitialize(string clientEmail, string subject)
        {
            var mail = new MailMessage();
            DefaultEncoding(mail);

            mail.To.Add(new MailAddress(clientEmail));
            MailAddress fromAddress = new MailAddress(config["From"]);
            mail.From = fromAddress;
            mail.Sender = fromAddress;
            mail.ReplyToList.Add(fromAddress);
            mail.Bcc.Add(new MailAddress(config["Bcc"]));

            mail.Subject = subject;

            mail.IsBodyHtml = true;

            return mail;
        }

        private void DefaultEncoding(MailMessage msg)
        {
            msg.BodyEncoding = Encoding.GetEncoding("utf-8");
            msg.SubjectEncoding = Encoding.GetEncoding("utf-8");
        }
    }
}
