using RIPharmStatutesAggregator.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RIPharmStatutesAggregator.Services
{
    public static class PageElementExtractor
    {
        const int tolerance = 10; // acceptable distance between tags on different lines or separating whitespace

        // tags
        static Tag BodyTag = new Tag("BODY");
        static Tag BoldTag = new Tag("B");
        static Tag ParagraphTag = new Tag("P");
        static Tag ItalicTag = new Tag("I");
        static Tag BreakTag = new Tag("BR");
        static Tag H1Tag = new Tag("H1");
        static Tag H2Tag = new Tag("H2");
        static Tag H3Tag = new Tag("H3");
        static Tag HistoryTag = new Tag("HISTORY");
        static Tag CenterTag = new Tag("CENTER");

        static string WILDCARD = ".*";
        static string NEWLINE = "\r\n";

        //var anyChar = "(?s:.*)"; // any char including newline
        static string[] separator = new string[] { BreakTag.Start };

        public static PageElements Extract(string html, bool isIndex)
        {
            var elements = new PageElements();

            ExtractTitleHeader(html, elements);
            ExtractChapterHeader(html, elements);
            ExtractArticleHeader(html, elements);
            ExtractHistoryFooter(html, elements);
            var contentStartIndex = ExtractSectionTitle(html, elements);
            ExtractSectionContents(html, elements, contentStartIndex);

            if(!isIndex)
                Validate(elements);
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
            else if (string.IsNullOrEmpty(elements.SectionTitle))
                throw new Exception("SectionTitle");
            else if (elements.SectionContents == null || elements.SectionContents.Count() < 1)
                throw new Exception("SectionContents");
        }

        private static void ExtractTitleHeader(string html, PageElements elements)
        {
            var titlePattern = H1Tag.Start + WILDCARD + H1Tag.End;
            var titleMatch = Regex.Match(html, titlePattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var titleCleaned = titleMatch.Value.Replace(H1Tag.Start, string.Empty).Replace(H1Tag.End, string.Empty).Replace(NEWLINE, string.Empty);
            var parts = titleCleaned.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1)
            {
                elements.TitleNumber = (parts[0] ?? string.Empty).Trim();
                elements.TitleName = (parts[1] ?? string.Empty).Trim();
            }
        }

        private static void ExtractChapterHeader(string html, PageElements elements)
        {
            var chapterPattern = H2Tag.Start + WILDCARD + H2Tag.End;
            var chapterMatch = Regex.Match(html, chapterPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var chapterCleaned = chapterMatch.Value.Replace(H2Tag.Start, string.Empty).Replace(H2Tag.End, string.Empty).Replace(NEWLINE, string.Empty);
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
            if (html.Contains(ItalicTag.Start))
            {
                // The first <I> contained within a an h2 header signifies an article heading
                // but sometimes the section body contains <I> tags and they can cause false positives without some kind of limit
                var anyCharsWithinLimit = GetAnyCharsWithinLimit(tolerance);

                var articleStartPattern = H2Tag.Start + anyCharsWithinLimit + ItalicTag.Start;
                var articleStartMatch = Regex.Match(html, articleStartPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var articleStartIndex = articleStartMatch.Index + articleStartMatch.Length;

                var articleEndPattern = ItalicTag.End + anyCharsWithinLimit + H2Tag.End;
                var articleEndMatch = Regex.Match(html, articleEndPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var articleEndIndex = articleEndMatch.Index - articleStartIndex;

                var articleContents = html.Substring(articleStartIndex, articleEndIndex);
                var articleCleaned = articleContents.Replace(ItalicTag.Start, string.Empty).Replace(ItalicTag.End, string.Empty).Replace(NEWLINE, string.Empty);

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
            if (html.Contains(HistoryTag.Start))
            {
                var historyPattern = HistoryTag.Start + WILDCARD + HistoryTag.End;
                var historyMatch = Regex.Match(html, historyPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var historyCleaned = historyMatch.Value.Replace(HistoryTag.Start, string.Empty)
                    .Replace(HistoryTag.End, string.Empty).Replace(NEWLINE, string.Empty);

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

            var startPattern = CenterTag.End + anyCharsWithinLimit + BreakTag.Start + anyCharsWithinLimit +
                ParagraphTag.Start + anyCharsWithinLimit + BoldTag.Start;
            var startMatch = Regex.Match(html, startPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var startIndex = (startMatch.Index + startMatch.Length);

            var endPattern = BoldTag.End + anyCharsWithinLimit + BreakTag.Start + anyCharsWithinLimit + BreakTag.Start;
            var endMatch = Regex.Match(html, endPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var endIndex = endMatch.Index;

            var secTitle = html.Substring(startIndex, endIndex - startIndex);
            var cleaned = secTitle.Replace(NEWLINE, string.Empty).Trim();
            elements.SectionTitle = cleaned;

            int contentStartIndex = endMatch.Index + endMatch.Length;
            return contentStartIndex;
        }

        private static void ExtractSectionContents(string html, PageElements elements, int bodyStartIndex = 0)
        {
            var bodyEndIndex = 0;
            if (html.Contains(HistoryTag.Start))
                bodyEndIndex = html.IndexOf(HistoryTag.Start);
            else if (html.Contains(BodyTag.End))
                bodyEndIndex = html.IndexOf(BodyTag.End);
            else
                bodyEndIndex = html.Length;

            var contents = html.Substring(bodyStartIndex, bodyEndIndex - bodyStartIndex);
            var cleaned = contents.Replace(BreakTag.Start, string.Empty);
            var lines = cleaned.Split(new string[] { NEWLINE }, StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < lines.Count(); i++)
            {
                lines[i] = lines[i].Replace(ParagraphTag.Start, string.Empty).Trim();
            }

            var splitLines = new List<Tuple<string, string>>();

            foreach(var line in lines)
            {
                var item1 = Regex.Match("(" + WILDCARD + ")", string.Empty).Value;
                var item2 = string.Empty;
                if(!string.IsNullOrWhiteSpace(item1))
                    item2 = line.Replace(item1, string.Empty);

                var tuple = new Tuple<string, string>(item1, item2);
                splitLines.Add(tuple);
            }
            elements.SectionContents = splitLines;
        }
    }
}
