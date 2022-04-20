using System.Collections.Generic;

namespace RainbowWine.Services.Email.Core.Email
{
    public interface ITemplateParser
    {
        string ReplaceTokens(string templateText, IDictionary<string, string> tokenValues);
    }
}