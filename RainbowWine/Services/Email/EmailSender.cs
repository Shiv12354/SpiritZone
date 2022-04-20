using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using System.Configuration;
using System.Collections.Specialized;
using RainbowWine.Services.Email.Core.Email;

namespace RainbowWine.Services.Email
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        NameValueCollection _config;
        public EmailSender()
        {
            _config = ConfigurationManager.AppSettings;
        }
        private SmtpClient GenerateSmtpClient()
        {
            var smtpClient = new SmtpClient(_config["SMTPServer"])
            {
                Port = 587,
                UseDefaultCredentials = false,
                EnableSsl = true,
                Credentials = new NetworkCredential(_config["From"],
                   _config["SMTPPassword"])
            };
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            return smtpClient;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Task.CompletedTask;
        }
        public Task SendEmailAsync(Dictionary<string, string> tokenValues, 
            string toEmail, string subject, string templatePath)
        {
            var factory = new EmailFactory(new TemplateParser());

           MailMessage message = factory
                .WithTokenValues(tokenValues)
                .WithSubject(subject)
                .WithHtmlBodyFromFile(templatePath)
                //.WithHtmlBodyFromFile(@"templates\html\request_qlik_user_to_register.html")
                //.WithPlainTextBodyFromFile(@"templates\sample-email.txt")
                .Create();

            var from = new System.Net.Mail.MailAddress(_config["From"], "SpiritZone");
            var to = new System.Net.Mail.MailAddress(toEmail);
            message.From = from;
            message.To.Add(to);

            GenerateSmtpClient().Send(message);
            return Task.CompletedTask;
        }
        //public Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        //{
        //    return emailSender.SendEmailAsync(email, "Confirm your email",
        //        $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        //}
        public bool SendChartOverMail(MailChartShareModel model)
        {
            try
            {
                string senderEmailId = _config["From"].ToString();
                MailMessage mailMessage = new MailMessage();
                MailAddress from = new MailAddress(senderEmailId); 
                foreach (var address in (model.Recipient).Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mailMessage.To.Add(address.Trim());
                }
                
                mailMessage.From = from;
                if(String.IsNullOrWhiteSpace(model.Cc) == false)
                {
                    foreach (var address in (model.Cc).Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        mailMessage.CC.Add(address.Trim());
                    }
                }
                if(String.IsNullOrWhiteSpace(model.Message) == false)
                    mailMessage.BodyEncoding = Encoding.UTF8;
                if(String.IsNullOrWhiteSpace(model.Subject) == false)
                {
                    mailMessage.Subject = model.Subject;
                    mailMessage.SubjectEncoding = Encoding.UTF8;
                }
                
                if(String.IsNullOrWhiteSpace(model.Message) == false)
                    mailMessage.Body = model.Message;
                if (model.Attachments != null)
                {
                    foreach (IFormFile attachment in model.Attachments)
                    {
                        if (attachment != null)
                        {
                            string fileName = Path.GetFileName(attachment.FileName);
                            mailMessage.Attachments.Add(new Attachment(attachment.OpenReadStream(), fileName));
                        }
                    }
                }
                mailMessage.Priority = MailPriority.High;
                mailMessage.IsBodyHtml = true;

                SmtpClient smtpClient = new SmtpClient(_config["SMTPServer"])
                {
                    Port = 587,
                    UseDefaultCredentials = false,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(_config["From"], _config["SMTPPassword"])
                };
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
