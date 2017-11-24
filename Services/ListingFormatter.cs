using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RIPharmStatutesAggregator.Core.StatuteElements;
using RIPharmStatutesAggregator.Core;

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
            html.AppendLine(Tags.html.Start);
            //header
            html.AppendLine(Tags.head.Start);
            html.AppendLine(Tags.style.Start);
            html.AppendLine(styleSheetProvider.GetStyleSheet());
            html.AppendLine(Tags.style.End);
            html.AppendLine(Tags.head.End);

            //body
            html.AppendLine(Tags.Body.Start);
            html.AppendLine(Tags.MakeDiv("title"));
            html.AppendLine(pageTitle);
            html.AppendLine(Tags.Div.End);
            html.AppendLine(Tags.MakeDiv("status"));
            html.AppendLine("Last Updated: " + DateTime.Now);
            html.AppendLine(Tags.Div.End);
            html.AppendLine(Tags.MakeDiv("content"));
            html.AppendLine(Tags.MakeDiv("page-index"));
            html.AppendLine(Tags.MakeParagraph("Page Index"));
            html.AppendLine(index);
            html.AppendLine(Tags.Div.End);
            html.AppendLine(body);
            html.AppendLine(Tags.Div.End);
            html.AppendLine(Tags.Body.End);
            html.AppendLine(Tags.html.End);
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
            html.Append(Tags.H1.Start);
            html.Append(title.TitleNumber);
            html.Append(Tags.Break.Start);
            html.Append(title.TitleName);
            html.Append(Tags.H1.End);

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
            html.Append(Tags.H2.Start);
            html.Append(chapter.ChapterNumber);
            html.Append(Tags.Break.Start);
            html.Append(chapter.ChapterName);
            html.Append(Tags.H2.End);

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
            html.Append(Tags.AnchorLink(section.SafeName));

            html.AppendLine();
            html.Append(Tags.H3.Start);
            html.Append("&sect; ");
            html.Append(section.SectionNumber);
            html.Append(" ");
            html.Append(section.SectionName);
            html.Append(Tags.H3.End);

            html.AppendLine();
            html.AppendLine(Tags.MakeDiv("section-body"));
            foreach (var line in section.Lines)
            {
                html.Append(Tags.Paragraph.Start);
                if(!string.IsNullOrWhiteSpace(line.Identifier))
                {
                    html.Append(Tags.Bold.Start);
                    html.Append("(");
                    html.Append(line.Identifier);
                    html.Append(")");
                    html.Append(Tags.Bold.End);
                    html.Append(" ");
                }
                html.Append(line.Contents);
                html.Append(Tags.Paragraph.End);
                html.AppendLine();
            }
            html.AppendLine(Tags.Div.End);
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
                historyList.AppendLine(Tags.List.Start);
                foreach (var entry in historyEntries)
                {
                    var entryMod = entry;
                    if (entry.EndsWith(".")) // remove unneeded period at end of list
                        entryMod = entry.Substring(0,entry.Length-1);
                    historyList.AppendLine(Tags.ListItem.Start);
                    historyList.AppendLine(entryMod.Replace("[SECT_SYMBOL]", "&sect;"));
                    historyList.AppendLine(Tags.ListItem.End);
                }
                historyList.Append(Tags.List.End);
            }

            var html = new StringBuilder();
            html.AppendLine(Tags.MakeDiv("section-history"));
            html.AppendLine(Tags.H4.Start);
            html.AppendLine("History of Section");
            html.AppendLine(Tags.H4.End);
            html.AppendLine(historyList.ToString());
            html.Append(Tags.Div.End);
            return html.ToString();
        }
    }
}
