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
        
        //var anyChar = "(?s:.*)"; // any char including newline
        static string[] separator = new string[] { "<BR>" };

        public static PageElements Extract(string html)
        {
            var elements = new PageElements();

            ExtractTitleHeader(html, elements);
            ExtractChapterHeader(html, elements);
            ExtractArticleHeader(html, elements);
            ExtractHistoryFooter(html, elements);
            var contentStartIndex = ExtractSectionTitle(html, elements);
            ExtractSectionContents(html, elements, contentStartIndex);

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
            var match = Regex.Match(html, H1Tag.Start + ".*" + H1Tag.End,
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var cleaned = match.Value.Replace(H1Tag.Start, "").Replace(H1Tag.End, "").Replace("\r\n", "");
            var split = cleaned.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length > 1)
            {
                elements.TitleNumber = (split[0] ?? "").Trim();
                elements.TitleName = (split[1] ?? "").Trim();
            }
        }

        private static void ExtractChapterHeader(string html, PageElements elements)
        {
            var match = Regex.Match(html, H2Tag.Start + ".*" + H2Tag.End,
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var cleaned = match.Value.Replace(H2Tag.Start, "").Replace(H2Tag.End, "").Replace("\r\n", "");
            var split = cleaned.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length > 1)
            {
                elements.ChapterNumber = (split[0] ?? "").Trim();
                elements.ChapterName = (split[1] ?? "").Trim();
            }
        }

        private static void ExtractArticleHeader(string html, PageElements elements)
        {
            if (html.Contains("<I>"))
            {
                // using tolerance here, since some pages use <I> tags elsewhere and can cause false positives without some kind of limit
                var startMatch = Regex.Match(html, H2Tag.Start + ".{0," + tolerance + "}" + ItalicTag.Start,
                    RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var articleStart = startMatch.Index + startMatch.Length;

                var endMatch = Regex.Match(html, ItalicTag.End + ".{0," + tolerance + "}" + H2Tag.End,
                    RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var articleContents = html.Substring(articleStart, endMatch.Index - articleStart);
                var cleaned = articleContents.Replace(ItalicTag.Start, "").Replace(ItalicTag.End, "").Replace("\r\n", "");

                var split = cleaned.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length > 1)
                {
                    elements.ArticleNumber = (split[0] ?? "").Trim();
                    elements.ArticleName = (split[1] ?? "").Trim();
                }
            }
        }

        private static void ExtractHistoryFooter(string html, PageElements elements)
        {
            if (html.Contains(HistoryTag.Start))
            {
                var historyMatch = Regex.Match(html, HistoryTag.Start + ".*" + HistoryTag.End,
                    RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var cleaned = historyMatch.Value.Replace(HistoryTag.Start, "").Replace(HistoryTag.End, "").Replace("\r\n", "");
                var split = cleaned.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length > 1)
                {
                    elements.HistoryHeader = (split[0] ?? "").Trim();
                    elements.HistoryList = (split[1] ?? "").Trim();
                    elements.HistoryList = elements.HistoryList.Replace("(", "").Replace(")", "");
                }
            }
        }

        private static int ExtractSectionTitle(string html, PageElements elements)
        {
            var anyChar = ".{0," + tolerance + "}";
            var startpattern = CenterTag.End + anyChar + BreakTag.Start + anyChar + ParagraphTag.Start + anyChar + BoldTag.Start;
            var startMatch = Regex.Match(html, startpattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var startindex = (startMatch.Index + startMatch.Length);

            var endMatch = Regex.Match(html, BoldTag.End + anyChar + BreakTag.Start + anyChar + BreakTag.Start,
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var endIndex = endMatch.Index;
            var secTitle = html.Substring(startindex, endIndex - startindex);
            var cleaned = secTitle.Replace("\r\n","").Trim();
            elements.SectionTitle = cleaned;

            int contentStartIndex = endMatch.Index + endMatch.Length;
            return contentStartIndex;
        }

        private static void ExtractSectionContents(string html, PageElements elements, int bodyStartIndex = 0)
        {
            var bodyEndIndex = 0;
            if (html.Contains(HistoryTag.Start))
                bodyEndIndex = html.IndexOf(HistoryTag.Start);
            else
                bodyEndIndex = html.IndexOf(BodyTag.End);

            var contents = html.Substring(bodyStartIndex, bodyEndIndex - bodyStartIndex);
            var cleaned = contents.Replace("<BR>", "");
            var lines = cleaned.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < lines.Count(); i++)
            {
                lines[i] = lines[i].Replace(ParagraphTag.Start, "").Trim();
            }

            elements.SectionContents = lines;
        }
    }
}
