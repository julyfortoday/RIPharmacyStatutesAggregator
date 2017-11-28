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
            var html = AddTopLevelHTML(listing);
            return html;
        }

        private string AddTopLevelHTML(StatuteListing listing)
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
            html.AppendLine(Tags.OpenDivWithId("page-title"));
            html.AppendLine(pageTitle);
            html.AppendLine(Tags.Div.End);
            html.AppendLine(Tags.OpenDivWithId("page-status"));
            html.AppendLine("Last Updated: " + DateTime.Now);
            html.AppendLine(Tags.Div.End);
            html.AppendLine(Tags.OpenDivWithId("page-index"));
            html.AppendLine(CreatePageIndex(listing));
            html.AppendLine(Tags.Div.End);
            html.AppendLine(Tags.OpenDivWithId("page-content"));
            html.Append(CreatePage(listing));
            html.AppendLine(Tags.Div.End);
            html.AppendLine();
            html.AppendLine(Tags.Body.End);
            html.AppendLine(Tags.html.End);
            return html.ToString();
        }

        private static string CreatePageIndex(StatuteListing listing)
        {
            var html = new StringBuilder();

            foreach (var title in listing.Titles)
            {                
                var titleLine = title.TitleNumber + " " + title.TitleName;
                html.AppendLine(Tags.H4.Start);
                html.AppendLine(Tags.AnchorLink(title.LinkID, titleLine));
                html.AppendLine(Tags.H4.End);
                var titleIndex = BuildTitleIndex(title);
                html.AppendLine(titleIndex);
            }

            return html.ToString();
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
            html.AppendLine(Tags.OpenDivWithClass("title-header"));
            html.Append(Tags.H1.Start);
            html.Append(Tags.Anchor(title.LinkID));
            html.Append(title.TitleNumber);
            html.Append(Tags.Break.Start);
            html.Append(title.TitleName);
            html.Append(Tags.H1.End);
            html.Append(BuildTitleIndex(title));
            html.AppendLine(Tags.Div.End);

            foreach (var chapter in title.Chapters)
            {
                var chapterHtml = FormatChaper(chapter);
                html.AppendLine(chapterHtml);
            }

            return html.ToString();
        }

        private static string BuildTitleIndex(Title title)
        {
            var html = new StringBuilder();
            html.AppendLine();
            html.AppendLine(Tags.List.Start);
            foreach (var chapter in title.Chapters)
            {
                html.AppendLine(Tags.ListItem.Start);
                html.AppendLine(Tags.AnchorLink(chapter.LinkID, chapter.ChapterNumber + " " + chapter.ChapterName));
                html.AppendLine(Tags.ListItem.End);
            }
            html.AppendLine(Tags.List.End);
            return html.ToString();
        }

        public static string FormatChaper(Chapter chapter)
        {
            var html = new StringBuilder();

            html.AppendLine();
            html.AppendLine(Tags.OpenDivWithClass("chapter-header"));
            html.AppendLine(Tags.Anchor(chapter.LinkID));
            html.Append("<H2 class=\"chapter-title\">");
            html.Append(chapter.ChapterNumber);
            html.Append(Tags.Break.Start);
            html.Append(chapter.ChapterName);
            html.Append(Tags.H2.End);
            html.AppendLine();
            html.AppendLine(Tags.MakeLink(chapter.OriginalLink, "(original)", "original-link"));
            html.AppendLine();
            html.Append(Tags.H5.Start);
            html.AppendLine(Tags.H5.End);
            html.Append(BuildChapterIndex(chapter));
            html.AppendLine(Tags.Div.End);

            if (chapter.Articles.Count < 1 )
            {
                foreach (var section in chapter.Sections)
                {
                    var sectionHtml = FormatSection(section);
                    html.AppendLine(sectionHtml);
                }
            }
            else
            {
                foreach (var article in chapter.Articles)
                {
                    html.Append(Tags.OpenDivWithClass("article-header"));
                    html.Append(Tags.H3.Start);
                    html.Append(article.ArticleNumber);
                    html.Append(" ");
                    html.AppendLine(article.ArticleName);
                    html.Append(Tags.H3.End);
                    html.Append(Tags.Div.End);

                    foreach (var section in article.Sections)
                    {
                        var sectionHtml = FormatSection(section);
                        html.AppendLine(sectionHtml);
                    }
                }
            }


            return html.ToString();
        }

        private static string BuildChapterIndex(Chapter chapter)
        {
            var html = new StringBuilder();

            html.AppendLine(Tags.OpenDivWithClass("chapter-index"));
            html.AppendLine(Tags.MakeParagraph("Index of Sections"));
            if (chapter.Articles.Count < 1)
            {
                html.AppendLine(Tags.List.Start);
                foreach (var section in chapter.Sections)
                {
                    html.AppendLine(Tags.ListItem.Start);
                    html.AppendLine(Tags.AnchorLink(section.LinkID, section.SectionNumber + " " + section.SectionName));
                    html.AppendLine(Tags.ListItem.End);
                }
                html.AppendLine(Tags.List.End);
            }
            else
            {
                foreach (var article in chapter.Articles)
                {
                    html.Append(Tags.OpenDivWithClass("index-article-heading"));
                    html.Append(article.ArticleNumber + " " + article.ArticleName);
                    html.Append(Tags.Div.End);
                    html.AppendLine(Tags.List.Start);
                    foreach (var section in article.Sections)
                    {
                        html.AppendLine(Tags.ListItem.Start);
                        html.AppendLine(Tags.AnchorLink(section.LinkID, section.SectionNumber + " " + section.SectionName));
                        html.AppendLine(Tags.ListItem.End);
                    }
                    html.AppendLine(Tags.List.End);
                }
            }
            html.AppendLine(Tags.Div.End);

            return html.ToString();
        }

        public static string FormatSection(Section section)
        {
            var html = new StringBuilder();

            html.AppendLine();
            html.AppendLine(Tags.OpenDivWithClass("section"));
            html.AppendLine(Tags.OpenDivWithClass("section-header"));
            html.AppendLine(Tags.Anchor(section.LinkID));
            html.Append("<H4 class=\"section-title\">");
            html.Append("&sect; ");
            html.Append(section.SectionNumber);
            html.Append(" ");
            html.Append(section.SectionName);
            html.Append(Tags.H4.End);
            html.AppendLine();
            html.AppendLine(Tags.MakeLink(section.OriginalLink, "(original)", "original-link"));
            html.AppendLine(Tags.Div.End);

            if (section.Lines != null && section.Lines.Count > 0)
            {
                html.Append(FormatSectionContents(section.Lines));
            }

            var footer = FormatSectionFooter(section.HistoryList);
            if(!string.IsNullOrWhiteSpace(footer))
            {
                html.Append(footer);
            }
            html.AppendLine();
            html.AppendLine(Tags.Div.End);

            return html.ToString();
        }

        private static string FormatSectionContents(List<Line> lines)
        {
            var html = new StringBuilder();

            html.AppendLine();
            html.AppendLine(Tags.OpenDivWithClass("section-body"));
            foreach (var line in lines)
            {
                html.Append(Tags.Paragraph.Start);
                if (!string.IsNullOrWhiteSpace(line.Identifier))
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

            return html.ToString();
        }

        private static string FormatSectionFooter(string historyList)
        {
            var formattedList = FormatHistoryList(historyList);
            var html = new StringBuilder();
            html.AppendLine();
            html.AppendLine(Tags.OpenDivWithClass("section-footer"));
            html.AppendLine(Tags.H5.Start);
            // not bother to get the original, this keeps it consistent in case
            html.AppendLine("History of Section");
            html.AppendLine(Tags.H5.End);
            html.AppendLine(formattedList);
            html.Append(Tags.Div.End);
            return html.ToString();
        }

        private static string FormatHistoryList(string historyList)
        {
            if (string.IsNullOrWhiteSpace(historyList))
                return string.Empty;

            var formattedList = new StringBuilder();
            var historyEntries = historyList.Replace("(", "").Replace(")", "").Replace("&sect;", "[SECT_SYMBOL]").Split(';');

            if (historyEntries.Count() > 0)
            {
                formattedList.AppendLine(Tags.List.Start);
                foreach (var entry in historyEntries)
                {
                    var entryMod = entry;
                    if (entry.EndsWith(".")) // remove unneeded period at end of list
                        entryMod = entry.Substring(0, entry.Length - 1);
                    formattedList.AppendLine(Tags.ListItem.Start);
                    formattedList.AppendLine(entryMod.Replace("[SECT_SYMBOL]", "&sect;"));
                    formattedList.AppendLine(Tags.ListItem.End);
                }
                formattedList.Append(Tags.List.End);
            }
            return formattedList.ToString();
        }
    }
}
