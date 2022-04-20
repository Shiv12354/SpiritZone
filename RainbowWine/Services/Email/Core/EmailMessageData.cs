using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RainbowWine.Services.Email.Core.Email
{
    public enum MailType
    {
        html,
        text
    }
    class EmailMessageData
    {
        public MailAddressCollection ToEmail { get; set; }
        public string FromEmail { get; set; }
        public string Subject { get; set; }
        public string BodyTemplate { get; set; }
        public Dictionary<string, string> TokenValues { get; set; }
        public MailType TemplateType { get; set; }
    }
}
