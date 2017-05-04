using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace OrderShare.Common
{
    public class RegexUtility
    {
        public static Regex CreateRegexWithInputName(string nameAttribute)
        {
            return new Regex(string.Concat("<input\\s[^>]*name=\"([\\w\\d$]*", nameAttribute, ")\"[^>]*>"), RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public static string ExtractHtmlContent(string tagName, string html)
        {
            Regex pattern = new Regex(string.Concat(".*<", tagName, "[^>]*>(.*)</", tagName, ">.*"), RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            return pattern.Replace(html, "$1");
        }

        public static string ExtractInputElementValue(string nameAttribute, string html)
        {
            Regex nameValueRegex = new Regex(string.Concat("<input\\s[^>]*name=\"[\\w\\d$]*", nameAttribute, "\".*value=\"(.*)\"[^>]*>"), RegexOptions.Compiled | RegexOptions.IgnoreCase);

            Match m = nameValueRegex.Match(html);
            if (m.Success)
                return m.Groups[1].Value;
            return string.Empty;
        }

        public static Regex GetHtmlObjectByRegex(string tagName)
        {
            return new Regex(String.Format(@"(?<{0}><{0}[^>]*>\s*([\S\s]*?)\s*</{0}>)", tagName),
                                RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }

        public static string ExtractHtmlContentByPattern(string sourceText, string matchString, string groupName = "")
        {
            string resultString = string.Empty;
            Regex regex = new Regex(matchString, RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Match match = regex.Match(sourceText);
            if (!string.IsNullOrEmpty(groupName))
            {
                resultString = match.Groups[groupName].Value;
                return resultString;
            }
            else
            {
                while (match.Success)
                {
                    for (int i = 0; i < match.Groups.Count; i++)
                    {
                        Group GroupObj = match.Groups[i];
                        if (GroupObj.Success)
                        {
                            resultString = match.Value;
                        }
                    }
                    match = match.NextMatch();
                }
            }
            return resultString;
        }

        public static string ExtractHyperlink(string sourceText)
        {
            string resultString = string.Empty;
            string matchString = "(?<=href=\")(?<link>.*?)(?=\")";
            Regex regex = new Regex(matchString, RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Multiline);
            MatchCollection matchs = regex.Matches(sourceText);
            Match match = null;
            if (matchs.Count > 0)
            {
                match = matchs[matchs.Count - 1];
            }

            if (match != null && match.Success && match.Groups["link"] != null)
            {
                resultString = match.Groups["link"].Value;
            }
            return resultString;
        }

        public static string ExtractTextFromHtmlTag(string html)
        {
            Regex regex = new Regex(@"(<.*?>)+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return regex.Replace(html, "");
        }

        public static bool ValidateEmail(string emailAddress)
        {
            return !string.IsNullOrEmpty(emailAddress)
                && System.Text.RegularExpressions.Regex.IsMatch(emailAddress,
                        @"\b[A-Za-z0-9._%-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}\b",
                        System.Text.RegularExpressions.RegexOptions.Compiled
                        | System.Text.RegularExpressions.RegexOptions.Singleline);
        }

        public static string ExtractViewState(string s)
        {
            string viewStateNameDelimiter = "__VIEWSTATE";
            string valueDelimiter = "value=\"";

            int viewStateNamePosition = s.IndexOf(viewStateNameDelimiter);
            if (viewStateNamePosition == -1)
                return string.Empty;

            int viewStateValuePosition = s.IndexOf(
                  valueDelimiter, viewStateNamePosition
               );
            int viewStateStartPosition = viewStateValuePosition +
                                         valueDelimiter.Length;
            int viewStateEndPosition = s.IndexOf("\"", viewStateStartPosition);
            return s.Substring(
                        viewStateStartPosition,
                        viewStateEndPosition - viewStateStartPosition
                  );
        }        
    }
}