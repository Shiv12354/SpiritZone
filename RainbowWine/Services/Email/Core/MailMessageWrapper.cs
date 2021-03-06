using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace RainbowWine.Services.Email.Core.Email
{
    public class MailMessageWrapper
    {
        protected internal MailMessage ContainedMailMessage;
        protected internal string HtmlBody;
        protected internal bool IsSubjectSet;
        protected internal string PlainTextBody;
        protected internal readonly ITemplateParser TemplateParser;
        protected internal IDictionary<string, string> TokenValues;

        public MailMessageWrapper(ITemplateParser templateParser)
        {
            ContainedMailMessage = new MailMessage();
            TemplateParser = templateParser;
        }

        public MailMessageWrapper WithSubject(string subjectTemplate)
        {
            if (IsSubjectSet)
                throw new InvalidOperationException("Subject has already been set");

            var populatedSubject = TemplateParser.ReplaceTokens(subjectTemplate, TokenValues);
            ContainedMailMessage.Subject = populatedSubject;
            IsSubjectSet = true;
            return this;
        }

        public MailMessageWrapper WithHtmlBody(string bodyTemplate)
        {
            if (HtmlBody != null)
                throw new InvalidOperationException("An HTML body already exists");

            string populatedBody = TemplateParser.ReplaceTokens(bodyTemplate, TokenValues);

            HtmlBody = populatedBody;
            return this;
        }

        public MailMessageWrapper WithPlainTextBody(string bodyTemplate)
        {
            if (PlainTextBody != null)
                throw new InvalidOperationException("A plaintext body already exists");

            string populatedBody = TemplateParser.ReplaceTokens(bodyTemplate, TokenValues);

            PlainTextBody = populatedBody;
            return this;
        }

        public MailMessageWrapper WithMarkdownBody(string bodyTemplate)
        {
            return WithPlainTextBody(bodyTemplate).WithHtmlBody(MarkdownHtmlGenerator.MarkdownToHtml(bodyTemplate));
        }

        public MailMessageWrapper WithMarkdownBodyFromFile(string filename)
        {
            return WithMarkdownBody(File.ReadAllText(filename));
        }

        public MailMessageWrapper WithHtmlBodyFromFile(string filename)
        {
            var path = Path.Combine(HttpContext.Current.Server.MapPath("~/"), filename);
            
            return WithHtmlBody(File.ReadAllText(path));
        }

        public MailMessageWrapper WithPlainTextBodyFromFile(string filename)
        {
            return WithPlainTextBody(File.ReadAllText(filename));
        }

        public MailMessage Create()
        {
            if (HtmlBody != null && PlainTextBody != null)
            {
                SetBodyFromPlainText();
                var htmlAlternative = AlternateView.CreateAlternateViewFromString(HtmlBody, null, MediaTypeNames.Text.Html);
                ContainedMailMessage.AlternateViews.Add(htmlAlternative);
            }
            else
            {
                if (HtmlBody != null)
                {
                    SetBodyFromHtmlText();
                }
                else if (PlainTextBody != null)
                {
                    SetBodyFromPlainText();
                }
            }

            return ContainedMailMessage;
        }

        protected void SetBodyFromPlainText()
        {
            ContainedMailMessage.Body = PlainTextBody;
            ContainedMailMessage.IsBodyHtml = false;
        }

        protected void SetBodyFromHtmlText()
        {
            ContainedMailMessage.Body = HtmlBody;
            ContainedMailMessage.IsBodyHtml = true;
        }

    }
}