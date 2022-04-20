
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RainbowWine.Services.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        bool SendChartOverMail(MailChartShareModel model);
        Task SendEmailAsync(Dictionary<string, string> tokenValues, string toEmail, string subject, string templatePath);
    }
}
