using RIPharmStatutesAggregator.Core;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace RIPharmStatutesAggregator.Services
{
    public class PageAggregator
    {
        private StyleSheetProvider styleSheetProvider;

        public PageAggregator(StyleSheetProvider styleSheetProvider)
        {
            this.styleSheetProvider = styleSheetProvider;
        }

        //<a href="#Section1.2">Section 1.2</a>
        //<a name="Section1.2"></a> 

        internal string Aggregate(List<Page> pages)
        {
            var listing = ListingBuilder.BuildListing(pages);
            var formatted = ListingFormatter.Format(listing);
            return formatted;
        }

        internal string Aggregate_Old(List<Page> pages)
        {
            var indexPages = pages.Where(x => x.Path.Contains("INDEX.HTM"));

            var aggregatedPage = new StringBuilder();
            foreach (var indexPage in indexPages)
            {
                var chapter = FormatChapter(indexPage, pages);
                aggregatedPage.Append(chapter);
            }

            var pageIndex = CreatePageIndex(pages);

            var html = AddTopLevelHTML(pageIndex, aggregatedPage.ToString());
            return html;
        }

        private string CreatePageIndex(List<Page> pages)
        {
            var indexPages = pages.Where(x => x.Path.Contains("INDEX.HTM"));
            var pageIndex = new StringBuilder();
            pageIndex.AppendLine("<ul>");
            foreach (var indexPage in indexPages)
            {
                var chapterName = Path.GetFileName(indexPage.Path).Replace("_INDEX.HTM", "");
                var subPages = pages.Where(x => x.Path.Contains(chapterName)).ToList();
                subPages.Remove(indexPage);
                var first = subPages.FirstOrDefault();

                var chapTitle = GetChapterTitle(first.Html, false);
                var cleaned = chapTitle.Replace("<H1>", "").Replace("</H1>", "").Replace("<BR>", " ").Replace("\r\n", "");

                pageIndex.AppendLine("<li><a href=\"#"+ chapTitle.Replace(" ", "_") + "\">"+ cleaned + "</a></li>");
            }
            pageIndex.AppendLine("</ul>");
            return pageIndex.ToString();
        }

        private string AddTopLevelHTML(string index, string body)
        {
            var pageTitle = "Rhode Island Pharmacy Statutes";

            var html = new StringBuilder();
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine("<style>");
            html.AppendLine(styleSheetProvider.GetStyleSheet());
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("<div id=\"title\">");
            html.AppendLine(pageTitle);
            html.AppendLine("</div>");
            html.AppendLine("<div id=\"status\">");
            html.AppendLine("Last Updated: " + DateTime.Now);
            html.AppendLine("</div>");
            html.AppendLine("<div id=\"content\">");
            html.AppendLine("<div id=\"page-index\">");
            html.AppendLine("<p>Page Index</p>");
            html.AppendLine(index);
            html.AppendLine("</div>");
            html.AppendLine(body);
            html.AppendLine("</div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            return html.ToString();
        }

        private string FormatChapter(Page indexPage, List<Page> pages)
        {          
            var chapterName = Path.GetFileName(indexPage.Path).Replace("_INDEX.HTM", "");
            var subPages = pages.Where(x => x.Path.Contains(chapterName)).ToList();
            subPages.Remove(indexPage);

            var first = subPages.FirstOrDefault();
            var chapTitle = GetChapterTitle(first.Html, false);
            var chapAnchor = "<a name=\"" + GetChapterTitle(first.Html, true).Replace(" ", "_") + "\" ></a> ";

            var sb = new StringBuilder();
            var index = new StringBuilder();
            index.Append("<h4>Index Of Sections</h4>");
            index.Append("<ul>");

            foreach (var page in subPages)
            {
                sb.Append(AdjustPage(page.Html, page));
                index.Append("<li><a href=\"#" + page.Elements.SectionTitle.Replace(" ", "_") + "\">"+ page.Elements.SectionTitle + "</a></li>");
            }
            index.Append("</ul>");

            return chapAnchor + chapTitle + index.ToString() + sb.ToString();
        }


        private string GetChapterTitle(string html, bool clean = true, bool useH2Tag = true)
        {
            var endTag = "</H1>";
            if (useH2Tag)
                endTag = "</H2>";
            var match = Regex.Match(html, "<H1>.*" + endTag, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var chapTitle = html.Substring(match.Index, match.Length);
            if(clean)
            {
                var cleaned = chapTitle
                    .Replace("<H1>", "")
                    .Replace("</H1>", "")
                    .Replace("<H2>", "")
                    .Replace("</H2>", "")
                    .Replace("<BR>", " ")
                    .Replace("\r\n", "");
                return cleaned;
            }
            return chapTitle;
        }

        private string AdjustPage(string html, Page page)
        {
            //var elements = PageElementExtractor.Extract(html);
            //page.Elements = elements;

            var header = FormatHeader(GetHeader(html));
            var body = "";
            var footer = "";
            if (!html.Contains("Repealed") && !html.Contains("Reserved"))
            {
                body = FormatBody(GetBody(html));
                footer = FormatFooter(GetFooter(html));
            }
            else
            {

            }

            var adjustedPage = "<div id=\"section\">\r\n" + header + body + footer + "</div>\r\n";

            return adjustedPage;
        }

        private string GetHeader(string html)
        {
            var headerStartIndex = html.IndexOf("<B>") + 3;
            var headerEndIndex = html.IndexOf("</B>");
            var headerText = html.Substring(headerStartIndex, headerEndIndex - headerStartIndex);
            return headerText.Trim();
        }

        private string FormatHeader(string header)
        {
            if (string.IsNullOrEmpty(header))
                return "";

            return "\t<h3 id=\"section-title\">" + header + "</h3>\r\n";
        }

        private string GetBody(string html)
        {
            var headerEndIndex = html.IndexOf("</B>");
            var bodyStartIndex = headerEndIndex + 4;
            var bodyEndIndex = html.IndexOf("<HISTORY>");
            var bodyText = html.Substring(bodyStartIndex, bodyEndIndex - bodyStartIndex);
            bodyText = bodyText.Replace("<BR>", "");
            bodyText = bodyText.Replace("<P>", "</P><P>");
            bodyText = bodyText.Replace("<P>\r\n", "\r\n<P>");
            return bodyText.Trim();
        }

        private string FormatBody(string body)
        {
            if (string.IsNullOrEmpty(body))
                return "";

            return "\t<div id=\"section-body\"><P>" + body + "</div>\r\n";
        }

        private string GetFooter(string html)
        {
            var footerStartIndex = html.IndexOf("<HISTORY>") + 9;
            var footerEndIndex = html.IndexOf("</HISTORY>");
            var footerText = html.Substring(footerStartIndex, footerEndIndex - footerStartIndex);
            footerText = footerText.Replace("<BR>", "").Replace("History of Section.", "");
            return footerText.Trim();
        }

        private string FormatFooter(string footer)
        {
            if (string.IsNullOrEmpty(footer))
                return "";

            var historyList = new StringBuilder();
            var historyEntries = footer.Replace("(", "").Replace(")", "").Replace("&sect;", "[SECT_SYMBOL]").Split(';');

            if(historyEntries.Count() > 0)
            {
                historyList.Append("\r\n\t\t<ul>\r\n");
                foreach (var entry in historyEntries)
                {
                    historyList.Append("\t\t\t<li>");
                    historyList.Append(entry.Replace("[SECT_SYMBOL]", "&sect;"));
                    historyList.Append("</li>\r\n");
                }
                historyList.Append("\t\t</ul>\r\n");
            }

            return "\t<div id=\"section-history\"><h4>History of Section</h4>" + historyList.ToString() + "\t</div>\r\n";
        }
    }
}
