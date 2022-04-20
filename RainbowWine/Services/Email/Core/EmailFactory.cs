// ****** https://github.com/endeavour/town-crier
//********https://thecodedecanter.wordpress.com/2010/07/19/town-crier-an-open-source-e-mail-templating-engine-for-net/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainbowWine.Services.Email.Core.Email
{
    public class EmailFactory
    {
        protected MailMessageWrapper Message;

        public EmailFactory(ITemplateParser templateParser)
        {
            Message = new MailMessageWrapper(templateParser);
        }

        public MailMessageWrapper WithTokenValues(IDictionary<string, string> tokenValues)
        {
            Message.TokenValues = tokenValues;
            return Message;
        }
    }
}
