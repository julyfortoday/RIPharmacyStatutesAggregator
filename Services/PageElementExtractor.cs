using RIPharmStatutesAggregator.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using static RIPharmStatutesAggregator.Core.Tags;

namespace RIPharmStatutesAggregator.Services
{
    public static class PageElementExtractor
    {
        const int tolerance = 10; // acceptable distance between tags on different lines or separating whitespace

        static string WILDCARD = ".*";
        static string NEWLINE = "\r\n";
        static string SECTION_SYMBOL = "&sect;";

        //var anyChar = "(?s:.*)"; // any char including newline

        public static PageElements Extract(string html, bool isIndex)
        {
            var elements = new PageElements();

            if (!isIndex)
            {
                ExtractTitleHeader(html, Tags.H1, elements);
                ExtractChapterHeader(html, Tags.H2, elements);
                ExtractArticleHeader(html, elements);
                ExtractHistoryFooter(html, elements);
                var contentStartIndex = ExtractSectionTitle(html, elements);
                ExtractSectionContents(html, elements, contentStartIndex);
                Validate(elements);
            }
            else
            {
                ExtractChapterHeader(html, Tags.H1, elements);
            }

            return elements;
        }

        public static void Validate(PageElements elements)
        {
            if (string.IsNullOrEmpty(elements.ChapterName))
                throw new Exception("ChapterName");
            else if (string.IsNullOrEmpty(elements.ChapterNumber))
                throw new Exception("ChapterNumber");
            else if (string.IsNullOrEmpty(elements.TitleName))
                throw new Exception("TitleName");
            else if (string.IsNullOrEmpty(elements.TitleNumber))
                throw new Exception("TitleNumber");
            else if (string.IsNullOrEmpty(elements.SectionNumber))
                throw new Exception("SectionTitle");
            else if (elements.SectionContents == null || elements.SectionContents.Count() < 1)
                throw new Exception("SectionContents");
        }

        private static void ExtractTitleHeader(string html, Tag tag,PageElements elements)
        {
            var titlePattern = tag.Start + WILDCARD + tag.End;
            var titleMatch = Regex.Match(html, titlePattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var titleCleaned = titleMatch.Value.Replace(tag.Start, string.Empty).Replace(tag.End, string.Empty).Replace(NEWLINE, string.Empty);

            var separator = new string[] { Tags.Break.Start };
            var parts = titleCleaned.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1)
            {
                elements.TitleNumber = (parts[0] ?? string.Empty).Trim();
                elements.TitleName = (parts[1] ?? string.Empty).Trim();
            }
        }

        private static void ExtractChapterHeader(string html, Tag tag, PageElements elements)
        {
            var chapterPattern = tag.Start + WILDCARD + tag.End;
            var chapterMatch = Regex.Match(html, chapterPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var chapterCleaned = chapterMatch.Value.Replace(tag.Start, string.Empty).Replace(tag.End, string.Empty).Replace(NEWLINE, string.Empty);

            var separator = new string[] { Tags.Break.Start };
            var parts = chapterCleaned.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1)
            {
                elements.ChapterNumber = (parts[0] ?? string.Empty).Trim();
                elements.ChapterName = (parts[1] ?? string.Empty).Trim();
            }
        }

        private static string GetAnyCharsWithinLimit(int limit)
        {
            var anyChar = ".{0," + limit + "}";
            return anyChar;
        }

        private static void ExtractArticleHeader(string html, PageElements elements)
        {
            if (html.Contains(Tags.Italic.Start))
            {
                // The first <I> contained within a an h2 header signifies an article heading
                // but sometimes the section body contains <I> tags and they can cause false positives without some kind of limit
                var anyCharsWithinLimit = GetAnyCharsWithinLimit(tolerance);

                var articleStartPattern = Tags.H2.Start + anyCharsWithinLimit + Tags.Italic.Start;
                var articleStartMatch = Regex.Match(html, articleStartPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var articleStartIndex = articleStartMatch.Index + articleStartMatch.Length;

                var articleEndPattern = Tags.Italic.End + anyCharsWithinLimit + Tags.H2.End;
                var articleEndMatch = Regex.Match(html, articleEndPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var articleEndIndex = articleEndMatch.Index - articleStartIndex;

                var articleContents = html.Substring(articleStartIndex, articleEndIndex);
                var articleCleaned = articleContents.Replace(Tags.Italic.Start, string.Empty).Replace(Tags.Italic.End, string.Empty).Replace(NEWLINE, string.Empty);

                var separator = new string[] { Tags.Break.Start };
                var parts = articleCleaned.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    elements.ArticleNumber = (parts[0] ?? string.Empty).Trim();
                    elements.ArticleName = (parts[1] ?? string.Empty).Trim();
                }
            }
        }

        private static void ExtractHistoryFooter(string html, PageElements elements)
        {
            if (html.Contains(Tags.HistoryTag.Start))
            {
                var historyPattern = Tags.HistoryTag.Start + WILDCARD + Tags.HistoryTag.End;
                var historyMatch = Regex.Match(html, historyPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var historyCleaned = historyMatch.Value.Replace(Tags.HistoryTag.Start, string.Empty)
                    .Replace(Tags.HistoryTag.End, string.Empty).Replace(NEWLINE, string.Empty);

                var separator = new string[] { Tags.Break.Start };
                var parts = historyCleaned.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    elements.HistoryHeader = (parts[0] ?? string.Empty).Trim();
                    elements.HistoryList = (parts[1] ?? string.Empty).Trim();
                    elements.HistoryList = elements.HistoryList.Replace("(", string.Empty).Replace(")", string.Empty);
                }
            }
        }

        private static int ExtractSectionTitle(string html, PageElements elements)
        {
            var anyCharsWithinLimit = GetAnyCharsWithinLimit(tolerance);

            var startPattern = Tags.CenterTag.End + anyCharsWithinLimit + Tags.Break.Start + anyCharsWithinLimit +
                Tags.Paragraph.Start + anyCharsWithinLimit + Tags.Bold.Start;
            var startMatch = Regex.Match(html, startPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var startIndex = (startMatch.Index + startMatch.Length);

            var endPattern = Tags.Bold.End + anyCharsWithinLimit + Tags.Break.Start + anyCharsWithinLimit + Tags.Break.Start;
            var endMatch = Regex.Match(html, endPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var endIndex = endMatch.Index;

            var secTitle = html.Substring(startIndex, endIndex - startIndex);
            var enDash = "&#150;";
            var cleaned = secTitle.Replace(NEWLINE, string.Empty).Replace(SECTION_SYMBOL, string.Empty).Replace(enDash,"-").Trim();
            if (cleaned.EndsWith("."))
                cleaned = cleaned.Substring(0,cleaned.Length-1);

            var splitIndex = cleaned.IndexOf(" ");
            if (splitIndex >= 0)
            {
                elements.SectionNumber = cleaned.Substring(0,splitIndex).Trim();
                elements.SectionName = cleaned.Substring(splitIndex,cleaned.Length - splitIndex).Trim();
            }

            int contentStartIndex = endMatch.Index + endMatch.Length;
            return contentStartIndex;
        }

        private static void ExtractSectionContents(string html, PageElements elements, int bodyStartIndex = 0)
        {
            if (html.Contains(Tags.List.Start))
            {
                return;
            }

            var bodyEndIndex = 0;
            if (html.Contains(Tags.HistoryTag.Start))
                bodyEndIndex = html.IndexOf(Tags.HistoryTag.Start);
            else if (html.Contains(Tags.Body.End))
                bodyEndIndex = html.IndexOf(Tags.Body.End);
            else
                bodyEndIndex = html.Length;

            var contents = html.Substring(bodyStartIndex, bodyEndIndex - bodyStartIndex);
            var cleaned = contents.Replace(Tags.Break.Start, string.Empty);
            var lines = cleaned.Split(new string[] { NEWLINE }, StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < lines.Count(); i++)
            {
                lines[i] = lines[i].Replace(Tags.Paragraph.Start, string.Empty).Trim();
            }

            var splitLines = new List<Tuple<string, string>>();

            foreach (var line in lines)
            {
                var item1 = string.Empty;
                var item2 = line;
                if (line.TrimStart().StartsWith("("))
                {
                    var openParen = line.IndexOf("(");
                    var closingParen = line.IndexOf(")");
                    item1 = line.Substring(openParen, closingParen + 1);
                    if (!string.IsNullOrWhiteSpace(item1))
                        item2 = line.Replace(item1, string.Empty);
                }

                var tuple = new Tuple<string, string>(item1.Replace("(", string.Empty).Replace(")", string.Empty), item2.Trim());
                splitLines.Add(tuple);
            }
            elements.SectionContents = splitLines;
        }
    }
}
