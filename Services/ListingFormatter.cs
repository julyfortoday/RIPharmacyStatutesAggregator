using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RIPharmStatutesAggregator.Core.StatuteElements;

namespace RIPharmStatutesAggregator.Services
{
    public class ListingFormatter
    {
        static string NEWLINE = "\r\n";
        private StyleSheetProvider styleSheetProvider;

        public ListingFormatter(StyleSheetProvider styleSheetProvider)
        {
            this.styleSheetProvider = styleSheetProvider;
        }

        internal string Format(StatuteListing listing)
        {
            var pageIndex = CreatePageIndex();
            var page = CreatePage(listing);
            var html = AddTopLevelHTML(pageIndex, page);
            return html;
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

        private static string CreatePageIndex()
        {
            return "Page Index";
        }

        private static string CreatePage(StatuteListing listing)
        {
            var html = new StringBuilder();

            foreach (var title in listing.Titles)
            {
                var titleHtml = FormatTitle(title);
                html.AppendLine(titleHtml);
            }

            return html.ToString();
        }

        public static string FormatTitle(Title title)
        {
            var html = new StringBuilder();

            html.AppendLine();
            html.Append("<h1>");
            html.Append(title.TitleNumber);
            html.Append("<br>");
            html.Append(title.TitleName);
            html.Append("</h1>");

            foreach (var chapter in title.Chapters)
            {
                var chapterHtml = FormatChaper(chapter);
                html.AppendLine(chapterHtml);
            }

            return html.ToString();
        }


        public static string FormatChaper(Chapter chapter)
        {
            var html = new StringBuilder();

            html.AppendLine();
            html.Append("<h2>");
            html.Append(chapter.ChapterNumber);
            html.Append("<br>");
            html.Append(chapter.ChapterName);
            html.Append("</h2>");

            foreach (var section in chapter.Sections)
            {
                var sectionHtml = FormatSection(section);
                html.AppendLine(sectionHtml);
            }

            return html.ToString();
        }

        //<a href="#Section1.2">Section 1.2</a>
        //<a name="Section1.2"></a> 

        public static string FormatSection(Section section)
        {
            var html = new StringBuilder();

            html.AppendLine();
            html.Append("<a name=\"#");
            html.Append(section.SafeName);
            html.Append("\"></a>");

            html.AppendLine();
            html.Append("<h3>");
            html.Append("&sect; ");
            html.Append(section.SectionNumber);
            html.Append(" ");
            html.Append(section.SectionName);
            html.Append("</h3>");

            html.AppendLine();
            html.AppendLine("<div id=\"section-body\">");
            foreach (var line in section.Lines)
            {
                html.Append("<p>");
                if(!string.IsNullOrWhiteSpace(line.Identifier))
                {
                    html.Append("<b>");
                    html.Append("(");
                    html.Append(line.Identifier);
                    html.Append(")");
                    html.Append("</b> ");
                }
                html.Append(line.Contents);
                html.Append("</p>");
                html.AppendLine();
            }
            html.AppendLine("</div>");
            html.AppendLine();

            var historyList = FormatHistoryList(section.HistoryList);
            if(!string.IsNullOrWhiteSpace(historyList))
            {
                //html.AppendLine(section.HistoryHeader);
                html.Append(historyList);
            }

            return html.ToString();
        }

        private static string FormatHistoryList(string list)
        {
            if (string.IsNullOrWhiteSpace(list))
                return string.Empty;
            
            var historyList = new StringBuilder();
            var historyEntries = list.Replace("(", "").Replace(")", "").Replace("&sect;", "[SECT_SYMBOL]").Split(';');

            if (historyEntries.Count() > 0)
            {
                historyList.AppendLine("<ul>");
                foreach (var entry in historyEntries)
                {
                    var entryMod = entry;
                    if (entry.EndsWith(".")) // remove unneeded period at end of list
                        entryMod = entry.Substring(0,entry.Length-1);
                    historyList.AppendLine("<li>");
                    historyList.AppendLine(entryMod.Replace("[SECT_SYMBOL]", "&sect;"));
                    historyList.AppendLine("</li>");
                }
                historyList.Append("</ul>");
            }

            var html = new StringBuilder();
            html.AppendLine("<div id=\"section-history\">");
            html.AppendLine("<h4>History of Section</h4>");
            html.AppendLine(historyList.ToString());
            html.Append("</div>");
            return html.ToString();
        }
    }
}
